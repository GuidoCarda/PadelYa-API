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

        /// <summary>
        /// Mapea un TournamentEnrollment a TournamentEnrollmentResponseDto con datos completos de los jugadores
        /// </summary>
        private async Task<TournamentEnrollmentResponseDto> MapEnrollmentToDto(TournamentEnrollment enrollment)
        {
            // Obtener los IDs de los jugadores de la pareja
            var playerIds = enrollment.Couple.Players.Select(p => p.Id).ToList();

            // Buscar los Users que tienen estos Players
            var users = await _context.Users
                .Include(u => u.Person)
                .Where(u => u.Person != null && playerIds.Contains(u.Person.Id))
                .ToListAsync();

            // Mapear los jugadores con sus datos completos
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

        /// <summary>
        /// Mapea un Tournament a TournamentResponseDto con enrollments y jugadores completos
        /// </summary>
        private async Task<TournamentResponseDto> MapTournamentToDto(Models.Tournament.Tournament tournament)
        {
            // Obtener todos los player IDs de todos los enrollments
            var allPlayerIds = tournament.Enrollments?
                .SelectMany(e => e.Couple.Players.Select(p => p.Id))
                .Distinct()
                .ToList() ?? new List<int>();

            // Buscar todos los Users de una vez para optimizar
            var allUsers = await _context.Users
                .Include(u => u.Person)
                .Where(u => u.Person != null && allPlayerIds.Contains(u.Person.Id))
                .ToListAsync();

            // Mapear enrollments
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
                TournamentPhases = new List<TournamentPhaseDto>() // Vacío por ahora
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

            // Mapear todos los torneos a DTOs
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
            //  OBTENER EL ID DEL USUARIO LOGUEADO DESDE EL TOKEN JWT
            var loggedInUserIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue("user_id");
            if (string.IsNullOrEmpty(loggedInUserIdString))
            {
                throw new ArgumentException("No se pudo identificar al usuario. Inicie sesión nuevamente.");
            }
            var loggedInUserId = int.Parse(loggedInUserIdString);
            var partnerId = enrollmentDto.PartnerId;

            //  VALIDAR QUE EL JUGADOR NO SE INSCRIBA CONSIGO MISMO
            if (loggedInUserId == partnerId)
            {
                throw new ArgumentException("No puedes inscribirte contigo mismo como compañero.");
            }

            //  BUSCAR EL TORNEO Y CARGAR LAS INSCRIPCIONES EXISTENTES
            var tournament = await _context.Tournaments
                .Include(t => t.Enrollments)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                throw new ArgumentException("El torneo especificado no existe.");
            }

            //  VALIDAR EL PERÍODO DE INSCRIPCIÓN (POR ESTADO Y FECHA)
            if (tournament.TournamentStatus != TournamentStatus.AbiertoParaInscripcion)
            {
                throw new ArgumentException("El torneo no está abierto para inscripciones.");
            }
            if (DateTime.UtcNow > tournament.EnrollmentEndDate)
            {
                throw new ArgumentException("El período de inscripción para este torneo ya ha finalizado.");
            }

            //  VALIDAR CUPOS DISPONIBLES
            if (tournament.Enrollments.Count >= tournament.Quota)
            {
                throw new ArgumentException("Los cupos para este torneo ya están llenos.");
            }

            //  VALIDAR QUE AMBOS USUARIOS (LOGUEADO Y COMPAÑERO) EXISTAN Y SEAN JUGADORES
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

            //  VERIFICAR QUE NINGUNO DE LOS DOS JUGADORES YA ESTÉ INSCRITO EN ESTE TORNEO
            var existingPlayerIdsInTournament = await _context.TournamentEnrollments
                .Where(e => e.TournamentId == tournamentId)
                .SelectMany(e => e.Couple.Players.Select(p => p.Id))
                .ToListAsync();

            if (existingPlayerIdsInTournament.Contains(loggedInUser.Person.Id) || existingPlayerIdsInTournament.Contains(partnerUser.Person.Id))
            {
                throw new ArgumentException("Uno de los jugadores ya se encuentra inscrito en este torneo.");
            }

            //  CREAR LA PAREJA (COUPLE)
            var newCouple = new Couple
            {
                Name = $"{loggedInUser.Name} & {partnerUser.Name}",
                Players = new List<Player> { (Player)loggedInUser.Person, (Player)partnerUser.Person }
            };
            _context.Couples.Add(newCouple); // Añadir la pareja al contexto para que se guarde

            //  CREAR LA INSCRIPCIÓN (ENROLLMENT)
            var newEnrollment = new TournamentEnrollment
            {
                TournamentId = tournament.Id,
                Couple = newCouple,
                CreatedAt = DateTime.UtcNow,
                EnrollmentDate = DateTime.UtcNow,
                PlayerId = loggedInUserId,
            };
            _context.TournamentEnrollments.Add(newEnrollment);

            // GUARDAR TODO EN LA BASE DE DATOS
            await _context.SaveChangesAsync();

            // RECARGAR EL ENROLLMENT CON TODOS LOS DATOS RELACIONADOS
            var enrollmentWithData = await _context.TournamentEnrollments
                .Include(e => e.Couple)
                    .ThenInclude(c => c.Players)
                .FirstOrDefaultAsync(e => e.Id == newEnrollment.Id);

            // Mapear a DTO con datos completos
            if (enrollmentWithData == null)
            {
                return null;
            }

            return enrollmentWithData;
        }

        public async Task<bool> CancelEnrollmentAsync(int tournamentId, int userId)
        {
            // OBTENER EL USUARIO PARA ACCEDER A SU PERSON ID
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Person == null)
            {
                throw new ArgumentException("Usuario no encontrado o no tiene perfil de jugador.");
            }

            // BUSCAR LA INSCRIPCIÓN DEL USUARIO EN EL TORNEO
            var enrollment = await _context.TournamentEnrollments
                .Include(e => e.Couple)
                .ThenInclude(c => c.Players)
                .Include(e => e.Tournament)
                .FirstOrDefaultAsync(e => e.TournamentId == tournamentId && 
                    e.Couple.Players.Any(p => p.Id == user.Person.Id));

            if (enrollment == null)
            {
                return false; // No se encontró la inscripción
            }

            //  VALIDAR QUE EL TORNEO ESTÉ ABIERTO PARA INSCRIPCIONES
            if (enrollment.Tournament.TournamentStatus != TournamentStatus.AbiertoParaInscripcion)
            {
                throw new ArgumentException("No se puede cancelar la inscripción. El torneo ya no está abierto para cambios.");
            }

            //  ELIMINAR LA INSCRIPCIÓN Y LA PAREJA ASOCIADA
            var couple = enrollment.Couple;
            
            _context.TournamentEnrollments.Remove(enrollment);
            _context.Couples.Remove(couple);

            //  GUARDAR LOS CAMBIOS
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
