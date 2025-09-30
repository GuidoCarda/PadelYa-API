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

namespace padelya_api.Services
{
    public class TournamentService(PadelYaDbContext context, IHttpContextAccessor httpContextAccessor) : ITournamentService
    {
        private readonly PadelYaDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

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
            var allPlayerIds = tournament.Enrollments?
                .SelectMany(e => e.Couple.Players.Select(p => p.Id))
                .Distinct()
                .ToList() ?? new List<int>();

            var allUsers = await _context.Users
                .Include(u => u.Person)
                .Where(u => u.Person != null && allPlayerIds.Contains(u.Person.Id))
                .ToListAsync();

            var enrollmentsDto = tournament.Enrollments?.Select(enrollment =>
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
                .Include(t => t.Enrollments)
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

            if (tournament.Enrollments.Count >= tournament.Quota)
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

            // Verificar que ninguno de los dos ya esté inscrito
            var existingPlayerIdsInTournament = await _context.TournamentEnrollments
                .Where(e => e.TournamentId == tournamentId)
                .SelectMany(e => e.Couple.Players.Select(p => p.Id))
                .ToListAsync();

            if (existingPlayerIdsInTournament.Contains(loggedInUser.Person.Id) || existingPlayerIdsInTournament.Contains(partnerUser.Person.Id))
            {
                throw new ArgumentException("Uno de los jugadores ya se encuentra inscrito en este torneo.");
            }

            // Crear pareja e inscripción
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
            };
            _context.TournamentEnrollments.Add(newEnrollment);

            await _context.SaveChangesAsync();

            // Recargar con datos relacionados para la respuesta
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
                    e.Couple.Players.Any(p => p.Id == user.Person.Id));

            if (enrollment == null)
            {
                return false;
            }

            // Solo permitir cancelación si el torneo sigue abierto
            if (enrollment.Tournament.TournamentStatus != TournamentStatus.AbiertoParaInscripcion)
            {
                throw new ArgumentException("No se puede cancelar la inscripción. El torneo ya no está abierto para cambios.");
            }

            var couple = enrollment.Couple;
            
            _context.TournamentEnrollments.Remove(enrollment);
            _context.Couples.Remove(couple);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
