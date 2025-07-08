using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Tournament;
using padelya_api.Models.Tournament;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public class TournamentService(PadelYaDbContext context) : ITournamentService
    {
        private readonly PadelYaDbContext _context = context;

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

        public async Task<List<Tournament>> GetTournamentsAsync()
        {
            var tournaments = await _context.Tournaments
                .Where(t => t.TournamentStatus != TournamentStatus.Deleted)
                .OrderByDescending(t => t.TournamentStartDate)
                .ToListAsync();

            return tournaments;
        }
        public async Task<bool> DeleteTournamentAsync(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);

            if (tournament == null)
            {
                return false;
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

            // Actualiza solo las propiedades que no son nulas en el DTO
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

        public async Task<Tournament?> GetTournamentByIdAsync(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            return tournament;
        }
    }
}
