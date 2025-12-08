using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Tournament;
using padelya_api.Models.Tournament;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using padelya_api.Models;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;

namespace padelya_api.Services
{
    public class TournamentService(PadelYaDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : ITournamentService
    {
        private readonly PadelYaDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IConfiguration _configuration = configuration;

        private async Task<TournamentEnrollmentResponseDto> MapEnrollmentToDto(TournamentEnrollment enrollment)
        {
            var playerIds = enrollment.Couple.Players.Select(p => p.Id).ToList();

            var users = await _context.Users
                .Include(u => u.Person)
                .Where(u => u.Person != null && playerIds.Contains(u.Person.Id))
                .ToListAsync();

            var playersDto = enrollment.Couple.Players.Select(player =>
            {
                var user = users.FirstOrDefault(u => u.Person?.Id == player.Id);
                return new PlayerResponseDto
                {
                    Id = player.Id,
                    Name = user?.Name ?? "",
                    Surname = user?.Surname ?? "",
                    Email = user?.Email ?? "",
                    Category = player.Category ?? ""
                };
            }).ToList();

            return new TournamentEnrollmentResponseDto
            {
                Id = enrollment.Id,
                TournamentId = enrollment.TournamentId,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status.ToString(),
                ExpiresAt = enrollment.ExpiresAt,
                Couple = new CoupleResponseDto
                {
                    Id = enrollment.Couple.Id,
                    Name = enrollment.Couple.Name,
                    Players = playersDto
                }
            };
        }

        private async Task<TournamentResponseDto> MapTournamentToDto(Models.Tournament.Tournament tournament)
        {
            var activeEnrollments = tournament.Enrollments?
                .Where(e => e.Status == TournamentEnrollmentStatus.Confirmed || 
                           (e.Status == TournamentEnrollmentStatus.PendingPayment && 
                            e.ExpiresAt.HasValue && e.ExpiresAt.Value > DateTime.UtcNow))
                .ToList() ?? new List<TournamentEnrollment>();

            var allPlayerIds = activeEnrollments
                .SelectMany(e => e.Couple.Players.Select(p => p.Id))
                .Distinct()
                .ToList();

            var allUsers = await _context.Users
                .Include(u => u.Person)
                .Where(u => u.Person != null && allPlayerIds.Contains(u.Person.Id))
                .ToListAsync();

            var enrollmentsDto = activeEnrollments.Select(enrollment =>
            {
                var playersDto = enrollment.Couple.Players.Select(player =>
                {
                    var user = allUsers.FirstOrDefault(u => u.Person?.Id == player.Id);
                    return new PlayerResponseDto
                    {
                        Id = player.Id,
                        Name = user?.Name ?? "",
                        Surname = user?.Surname ?? "",
                        Email = user?.Email ?? "",
                        Category = player.Category ?? ""
                    };
                }).ToList();

                return new TournamentEnrollmentResponseDto
                {
                    Id = enrollment.Id,
                    TournamentId = enrollment.TournamentId,
                    EnrollmentDate = enrollment.EnrollmentDate,
                    Status = enrollment.Status.ToString(),
                    ExpiresAt = enrollment.ExpiresAt,
                    Couple = new CoupleResponseDto
                    {
                        Id = enrollment.Couple.Id,
                        Name = enrollment.Couple.Name,
                        Players = playersDto
                    }
                };
            }).ToList() ?? new List<TournamentEnrollmentResponseDto>();


            return new TournamentResponseDto
            {
                Id = tournament.Id,
                CurrentPhase = tournament.CurrentPhase,
                TournamentStatus = tournament.TournamentStatus,
                Title = tournament.Title,
                Category = tournament.Category,
                Quota = tournament.Quota,
                EnrollmentPrice = tournament.EnrollmentPrice,
                EnrollmentStartDate = tournament.EnrollmentStartDate,
                EnrollmentEndDate = tournament.EnrollmentEndDate,
                TournamentStartDate = tournament.TournamentStartDate,
                TournamentEndDate = tournament.TournamentEndDate,
                Enrollments = enrollmentsDto,
                TournamentPhases = new List<TournamentPhaseDto>()
            };
        }

        public async Task<Tournament?> CreateTournamentAsync(CreateTournamentDto tournamentDto)
        {
            if (tournamentDto.EnrollmentEndDate <= tournamentDto.EnrollmentStartDate)
            {
                throw new ArgumentException("La fecha de fin de inscripciones debe ser posterior a la de inicio.");
            }
            if (tournamentDto.TournamentEndDate <= tournamentDto.TournamentStartDate)
            {
                throw new ArgumentException("La fecha de fin del torneo debe ser posterior a la de inicio.");
            }

            var tournament = new Tournament
            {
                Title = tournamentDto.Title,
                Category = tournamentDto.Category,
                Quota = tournamentDto.Quota,
                EnrollmentPrice = tournamentDto.EnrollmentPrice,
                EnrollmentStartDate = tournamentDto.EnrollmentStartDate,
                EnrollmentEndDate = tournamentDto.EnrollmentEndDate,
                TournamentStartDate = tournamentDto.TournamentStartDate,
                TournamentEndDate = tournamentDto.TournamentEndDate,
                TournamentStatus = TournamentStatus.AbiertoParaInscripcion,
                CurrentPhase = "Inscripción",
                Enrollments = new List<TournamentEnrollment>(),
                TournamentPhases = new List<TournamentPhase>()
            };

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();
            return tournament;
        }

        public async Task<List<TournamentResponseDto>> GetTournamentsAsync()
        {
            await CleanupAllExpiredEnrollmentsAsync();

            var tournaments = await _context.Tournaments
                .Include(t => t.Enrollments)           
                .ThenInclude(e => e.Couple)           
                .ThenInclude(c => c.Players)
                .Where(t => t.TournamentStatus != TournamentStatus.Deleted)
                .OrderByDescending(t => t.TournamentStartDate)
                .ToListAsync();

            var tournamentsDto = new List<TournamentResponseDto>();
            foreach (var tournament in tournaments)
            {
                tournamentsDto.Add(await MapTournamentToDto(tournament));
            }

            return tournamentsDto;
        }

        public async Task<bool> DeleteTournamentAsync(int id)
        {
            var tournament = await _context.Tournaments.Include(t => t.Enrollments)
                                                      .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return false;
            }

            if (tournament.Enrollments.Any())
            {
                throw new ArgumentException("No se puede eliminar un torneo que ya tiene inscripciones.");
            }

            tournament.TournamentStatus = TournamentStatus.Deleted;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Tournament?> UpdateTournamentAsync(int id, UpdateTournamentDto updateDto)
        {
            var tournament = await _context.Tournaments.FindAsync(id);

            if (tournament == null)
            {
                return null;
            }

            if (tournament.TournamentStatus != TournamentStatus.AbiertoParaInscripcion)
            {
                throw new ArgumentException("Solo se pueden actualizar torneos que están abiertos para inscripción.");
            }

            if (updateDto.Title != null)
                tournament.Title = updateDto.Title;

            if (updateDto.Category != null)
                tournament.Category = updateDto.Category;

            if (updateDto.Quota.HasValue)
                tournament.Quota = updateDto.Quota.Value;

            if (updateDto.EnrollmentPrice.HasValue)
                tournament.EnrollmentPrice = updateDto.EnrollmentPrice.Value;

            if (updateDto.EnrollmentStartDate.HasValue)
                tournament.EnrollmentStartDate = updateDto.EnrollmentStartDate.Value;

            if (updateDto.EnrollmentEndDate.HasValue)
                tournament.EnrollmentEndDate = updateDto.EnrollmentEndDate.Value;

            if (updateDto.TournamentStartDate.HasValue)
                tournament.TournamentStartDate = updateDto.TournamentStartDate.Value;

            if (updateDto.TournamentEndDate.HasValue)
                tournament.TournamentEndDate = updateDto.TournamentEndDate.Value;

            await _context.SaveChangesAsync();
            return tournament;
        }

        public async Task<TournamentResponseDto?> GetTournamentByIdAsync(int id)
        {
            await CleanupExpiredEnrollmentsAsync(id);

            var tournament = await _context.Tournaments
                .Include(t => t.Enrollments)
                    .ThenInclude(e => e.Couple)
                        .ThenInclude(c => c.Players)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return null;
            }
            
            return await MapTournamentToDto(tournament);
        }

        public async Task<Tournament?> UpdateTournamentStatusAsync(int id, TournamentStatus newStatus)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return null;
            }

        
            if (tournament.TournamentStatus == TournamentStatus.Finalizado)
            {
                throw new ArgumentException("No se puede cambiar el estado de un torneo que ya está finalizado.");
            }

            tournament.TournamentStatus = newStatus;
            await _context.SaveChangesAsync();
            return tournament;

        }

        public async Task<TournamentEnrollment?> EnrollPlayerAsync(int tournamentId, TournamentEnrollmentDto enrollmentDto)
        {
            var loggedInUserIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue("user_id");
            if (string.IsNullOrEmpty(loggedInUserIdString))
            {
                throw new ArgumentException("No se pudo identificar al usuario. Inicie sesión nuevamente.");
            }
            var loggedInUserId = int.Parse(loggedInUserIdString);
            var partnerId = enrollmentDto.PartnerId;

            if (loggedInUserId == partnerId)
            {
                throw new ArgumentException("No puedes inscribirte contigo mismo como compañero.");
            }

            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                throw new ArgumentException("El torneo especificado no existe.");
            }

            // Validar estado y período de inscripción
            if (tournament.TournamentStatus != TournamentStatus.AbiertoParaInscripcion)
            {
                throw new ArgumentException("El torneo no está abierto para inscripciones.");
            }
            if (DateTime.UtcNow > tournament.EnrollmentEndDate)
            {
                throw new ArgumentException("El período de inscripción para este torneo ya ha finalizado.");
            }

            // Limpiar inscripciones expiradas antes de validar cupos
            await CleanupExpiredEnrollmentsAsync(tournamentId);

            // Hacer consulta fresca para contar inscripciones confirmadas (no depender de colecciones en memoria)
            var confirmedEnrollmentsCount = await _context.TournamentEnrollments
                .CountAsync(e => e.TournamentId == tournamentId && 
                                e.Status == TournamentEnrollmentStatus.Confirmed);

            if (confirmedEnrollmentsCount >= tournament.Quota)
            {
                throw new ArgumentException("Los cupos para este torneo ya están llenos.");
            }

            // Cargar ambos usuarios (el logueado y su compañero)
            var playersToEnroll = await _context.Users
                .Where(u => u.Id == loggedInUserId || u.Id == partnerId)
                .Include(u => u.Person)
                .ToListAsync();

            if (playersToEnroll.Count != 2)
            {
                throw new ArgumentException("Uno o ambos jugadores no existen en el sistema.");
            }

            var loggedInUser = playersToEnroll.First(u => u.Id == loggedInUserId);
            var partnerUser = playersToEnroll.First(u => u.Id == partnerId);

            if (loggedInUser.Person == null || partnerUser.Person == null)
            {
                throw new ArgumentException("Ambos usuarios deben tener un perfil de jugador para inscribirse.");
            }

            var existingEnrollments = await _context.TournamentEnrollments
                .Include(e => e.Couple)
                .ThenInclude(c => c.Players)
                .Where(e => e.TournamentId == tournamentId && 
                           (e.Status == TournamentEnrollmentStatus.Confirmed || 
                           (e.Status == TournamentEnrollmentStatus.PendingPayment && 
                            e.ExpiresAt.HasValue && e.ExpiresAt.Value > DateTime.UtcNow)))
                .ToListAsync();

            var existingPlayerIds = existingEnrollments
                .SelectMany(e => e.Couple.Players.Select(p => p.Id))
                .ToList();

            if (existingPlayerIds.Contains(loggedInUser.Person.Id) || existingPlayerIds.Contains(partnerUser.Person.Id))
            {
                throw new ArgumentException("Uno de los jugadores ya se encuentra inscrito o tiene una inscripción pendiente en este torneo.");
            }

            var newCouple = new Couple
            {
                Name = $"{loggedInUser.Name} & {partnerUser.Name}",
                Players = new List<Player> { (Player)loggedInUser.Person, (Player)partnerUser.Person }
            };
            _context.Couples.Add(newCouple);

            var newEnrollment = new TournamentEnrollment
            {
                TournamentId = tournament.Id,
                Couple = newCouple,
                CreatedAt = DateTime.UtcNow,
                EnrollmentDate = DateTime.UtcNow,
                PlayerId = loggedInUserId,
                Status = TournamentEnrollmentStatus.Confirmed,
                ExpiresAt = null
            };
            _context.TournamentEnrollments.Add(newEnrollment);

            await _context.SaveChangesAsync();

            var enrollmentWithData = await _context.TournamentEnrollments
                .Include(e => e.Couple)
                    .ThenInclude(c => c.Players)
                .FirstOrDefaultAsync(e => e.Id == newEnrollment.Id);

            if (enrollmentWithData == null)
            {
                return null;
            }

            return enrollmentWithData;
        }

        public async Task<TournamentEnrollmentInitPointDto> EnrollWithPaymentAsync(int tournamentId, TournamentEnrollmentWithPaymentDto enrollmentDto)
        {
            var loggedInUserIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue("user_id");
            if (string.IsNullOrEmpty(loggedInUserIdString))
            {
                throw new ArgumentException("No se pudo identificar al usuario. Inicie sesión nuevamente.");
            }
            var loggedInUserId = int.Parse(loggedInUserIdString);
            var partnerId = enrollmentDto.PartnerId;

            if (loggedInUserId == partnerId)
            {
                throw new ArgumentException("No puedes inscribirte contigo mismo como compañero.");
            }

            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                throw new ArgumentException("El torneo especificado no existe.");
            }

            // Validar estado y período de inscripción
            if (tournament.TournamentStatus != TournamentStatus.AbiertoParaInscripcion)
            {
                throw new ArgumentException("El torneo no está abierto para inscripciones.");
            }
            if (DateTime.UtcNow > tournament.EnrollmentEndDate)
            {
                throw new ArgumentException("El período de inscripción para este torneo ya ha finalizado.");
            }

            // Limpiar inscripciones expiradas antes de validar cupos
            await CleanupExpiredEnrollmentsAsync(tournamentId);

            // Cancelar inscripciones pendientes del usuario actual (permite reintentar inmediatamente)
            await CancelUserPendingEnrollmentsAsync(tournamentId, loggedInUserId);

            // Hacer consulta fresca para contar inscripciones confirmadas (no depender de colecciones en memoria)
            var confirmedEnrollmentsCount = await _context.TournamentEnrollments
                .CountAsync(e => e.TournamentId == tournamentId && 
                                e.Status == TournamentEnrollmentStatus.Confirmed);

            if (confirmedEnrollmentsCount >= tournament.Quota)
            {
                throw new ArgumentException("Los cupos para este torneo ya están llenos.");
            }

            // Cargar ambos usuarios (el logueado y su compañero)
            var playersToEnroll = await _context.Users
                .Where(u => u.Id == loggedInUserId || u.Id == partnerId)
                .Include(u => u.Person)
                .ToListAsync();

            if (playersToEnroll.Count != 2)
            {
                throw new ArgumentException("Uno o ambos jugadores no existen en el sistema.");
            }

            var loggedInUser = playersToEnroll.First(u => u.Id == loggedInUserId);
            var partnerUser = playersToEnroll.First(u => u.Id == partnerId);

            if (loggedInUser.Person == null || partnerUser.Person == null)
            {
                throw new ArgumentException("Ambos usuarios deben tener un perfil de jugador para inscribirse.");
            }

            // Verificar que ninguno de los dos ya esté inscrito (solo inscripciones ACTIVAS: confirmadas o pendientes válidas)
            var activeEnrollments = await _context.TournamentEnrollments
                .Include(e => e.Couple)
                .ThenInclude(c => c.Players)
                .Where(e => e.TournamentId == tournamentId && 
                           (e.Status == TournamentEnrollmentStatus.Confirmed || 
                           (e.Status == TournamentEnrollmentStatus.PendingPayment && 
                            e.ExpiresAt.HasValue && e.ExpiresAt.Value > DateTime.UtcNow)))
                .ToListAsync();

            var activePlayerIds = activeEnrollments
                .SelectMany(e => e.Couple.Players.Select(p => p.Id))
                .ToList();

            if (activePlayerIds.Contains(loggedInUser.Person.Id) || activePlayerIds.Contains(partnerUser.Person.Id))
            {
                throw new ArgumentException("Uno de los jugadores ya se encuentra inscrito o tiene una inscripción pendiente válida en este torneo.");
            }

            // Crear pareja e inscripción con estado PendingPayment
            var newCouple = new Couple
            {
                Name = $"{loggedInUser.Name} & {partnerUser.Name}",
                Players = new List<Player> { (Player)loggedInUser.Person, (Player)partnerUser.Person }
            };
            _context.Couples.Add(newCouple);

            var newEnrollment = new TournamentEnrollment
            {
                TournamentId = tournament.Id,
                Couple = newCouple,
                CreatedAt = DateTime.UtcNow,
                EnrollmentDate = DateTime.UtcNow,
                PlayerId = loggedInUserId,
                Status = TournamentEnrollmentStatus.PendingPayment,
                PreferenceId = null,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };
            _context.TournamentEnrollments.Add(newEnrollment);
            await _context.SaveChangesAsync();

            var preference = await CreateMercadoPagoPreference(newEnrollment, tournament.EnrollmentPrice, loggedInUserId);

            if (preference == null)
                throw new Exception("Preference not created");

            newEnrollment.PreferenceId = preference.Id;
            await _context.SaveChangesAsync();

            var response = new TournamentEnrollmentInitPointDto
            {
                init_point = preference.InitPoint
            };

            return response;
        }

        private async Task<Preference> CreateMercadoPagoPreference(TournamentEnrollment enrollment, decimal amount, int personId)
        {
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];

            var externalReference = $"tournament_{enrollment.TournamentId}_enrollment_{enrollment.Id}";

            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = $"Inscripción al torneo",
                        Quantity = 1,
                        CurrencyId = "ARS",
                        UnitPrice = amount
                    }
                },
                ExternalReference = externalReference,
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://x8dgxb6z-3000.brs.devtunnels.ms/tournaments/payment/success",
                    Failure = "https://x8dgxb6z-3000.brs.devtunnels.ms/tournaments/payment/failure",
                    Pending = "https://x8dgxb6z-3000.brs.devtunnels.ms/tournaments/payment/pending"
                },
                NotificationUrl = "https://x8dgxb6z-5105.brs.devtunnels.ms/api/Payments/webhook",
                Metadata = new Dictionary<string, object>
                {
                    ["tournament_enrollment_id"] = enrollment.Id.ToString(),
                    ["person_id"] = personId.ToString(),
                    ["tournament_id"] = enrollment.TournamentId.ToString()
                },
                AutoReturn = "approved",
                Expires = true,
                ExpirationDateTo = DateTime.UtcNow.AddMinutes(15),
                DateOfExpiration = DateTime.UtcNow.AddMinutes(15),
                PaymentMethods = new()
                {
                    Installments = 1,
                    ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                    {
                        new PreferencePaymentTypeRequest
                        {
                            Id = "ticket",
                        }
                    }
                }
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(request);

            return preference;
        }
        private async Task CleanupExpiredEnrollmentsAsync(int tournamentId)
        {
            var expiredEnrollments = await _context.TournamentEnrollments
                .Include(e => e.Couple)
                .Where(e => e.TournamentId == tournamentId &&
                           e.Status == TournamentEnrollmentStatus.PendingPayment &&
                           e.ExpiresAt.HasValue &&
                           e.ExpiresAt.Value <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredEnrollments.Any())
            {
                foreach (var enrollment in expiredEnrollments)
                {
                    enrollment.Status = TournamentEnrollmentStatus.Cancelled;
                }
                await _context.SaveChangesAsync();
            }
        }

        private async Task CleanupAllExpiredEnrollmentsAsync()
        {
            var expiredEnrollments = await _context.TournamentEnrollments
                .Where(e => e.Status == TournamentEnrollmentStatus.PendingPayment &&
                           e.ExpiresAt.HasValue &&
                           e.ExpiresAt.Value <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredEnrollments.Any())
            {
                foreach (var enrollment in expiredEnrollments)
                {
                    enrollment.Status = TournamentEnrollmentStatus.Cancelled;
                }
                await _context.SaveChangesAsync();
            }
        }

        private async Task CancelUserPendingEnrollmentsAsync(int tournamentId, int userId)
        {
            // Buscar inscripciones pendientes del usuario (independientemente de si están expiradas o no)
            var pendingEnrollments = await _context.TournamentEnrollments
                .Where(e => e.TournamentId == tournamentId &&
                           e.PlayerId == userId &&
                           e.Status == TournamentEnrollmentStatus.PendingPayment)
                .ToListAsync();

            if (pendingEnrollments.Any())
            {
                foreach (var enrollment in pendingEnrollments)
                {
                    enrollment.Status = TournamentEnrollmentStatus.Cancelled;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CancelEnrollmentAsync(int tournamentId, int userId)
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Person == null)
            {
                throw new ArgumentException("Usuario no encontrado o no tiene perfil de jugador.");
            }

            var enrollment = await _context.TournamentEnrollments
                .Include(e => e.Couple)
                .ThenInclude(c => c.Players)
                .Include(e => e.Tournament)
                .FirstOrDefaultAsync(e => e.TournamentId == tournamentId && 
                    e.Couple.Players.Any(p => p.Id == user.Person.Id) &&
                    (e.Status == TournamentEnrollmentStatus.Confirmed || 
                     e.Status == TournamentEnrollmentStatus.PendingPayment));

            if (enrollment == null)
            {
                return false;
            }

            if (enrollment.Tournament.TournamentStatus != TournamentStatus.AbiertoParaInscripcion)
            {
                throw new ArgumentException("No se puede cancelar la inscripción. El torneo ya no está abierto para cambios.");
            }

            enrollment.Status = TournamentEnrollmentStatus.Cancelled;
            _context.Entry(enrollment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            var saveResult = await _context.SaveChangesAsync();

            if (saveResult == 0)
            {
                throw new Exception("No se pudo guardar el cambio de estado de la inscripción");
            }

            return true;
        }
    }
}
