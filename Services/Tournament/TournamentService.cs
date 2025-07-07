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
    }
}