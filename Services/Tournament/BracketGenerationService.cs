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
    public class BracketGenerationService(PadelYaDbContext context) : IBracketGenerationService
    {
        private readonly PadelYaDbContext _context = context;

        public async Task<GenerateBracketResponseDto?> GenerateTournamentBracketAsync(int tournamentId)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Enrollments)
                .ThenInclude(e => e.Couple)
                .ThenInclude(c => c.Players)
                .Include(t => t.TournamentPhases)
                .ThenInclude(p => p.Brackets)
                .ThenInclude(b => b.Matches)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                return null;
            }

            // Validar que las inscripciones estén cerradas
            if (tournament.TournamentStatus != TournamentStatus.AbiertoParaInscripcion)
            {
                throw new ArgumentException("El torneo debe estar en estado 'Abierto para Inscripción' para generar el bracket.");
            }

            var enrolledCouples = tournament.Enrollments.Select(e => e.Couple).ToList();
            var totalPlayers = enrolledCouples.Sum(c => c.Players.Count);

            if (enrolledCouples.Count < 2 || totalPlayers < 4)
            {
                throw new ArgumentException("Se necesitan al menos 4 jugadores (2 parejas) para generar el cuadro. Un torneo de pádel requiere mínimo una final con 2 parejas.");
            }

            // Eliminar brackets existentes si los hay
            if (tournament.TournamentPhases.Any())
            {
                var existingPhases = tournament.TournamentPhases.ToList();
                foreach (var phase in existingPhases)
                {
                    var brackets = phase.Brackets.ToList();
                    foreach (var bracket in brackets)
                    {
                        _context.TournamentMatches.RemoveRange(bracket.Matches);
                    }
                    _context.TournamentBrackets.RemoveRange(brackets);
                }
                _context.TournamentPhases.RemoveRange(existingPhases);
                await _context.SaveChangesAsync();
            }

            // Determinar el tamaño del bracket (potencia de 2 más cercana)
            int bracketSize = GetNextPowerOfTwo(enrolledCouples.Count);

            // Calcular número de rondas
            int numberOfRounds = (int)Math.Log2(bracketSize);

            // Generar las fases del torneo
            var phases = new List<TournamentPhase>();
            var currentDate = tournament.TournamentStartDate;
            var daysPerPhase = (tournament.TournamentEndDate - tournament.TournamentStartDate).Days / numberOfRounds;

            for (int round = numberOfRounds; round >= 1; round--)
            {
                string phaseName = GetPhaseName(round);
                var phaseStartDate = currentDate;
                var phaseEndDate = currentDate.AddDays(daysPerPhase);

                var phase = new TournamentPhase
                {
                    TournamentId = tournamentId,
                    PhaseName = phaseName,
                    StartDate = phaseStartDate,
                    EndDate = phaseEndDate,
                    Brackets = new List<TournamentBracket>()
                };

                phases.Add(phase);
                currentDate = phaseEndDate;
            }

            // Guardar las fases primero para obtener IDs
            _context.TournamentPhases.AddRange(phases);
            await _context.SaveChangesAsync();

            // Generar los brackets y matches
            int totalMatches = 0;

            // Mezclar aleatoriamente las parejas para un sorteo justo
            var shuffledCouples = enrolledCouples.OrderBy(x => Guid.NewGuid()).ToList();

            // Generar la primera ronda con las parejas
            var firstPhase = phases[0];
            var firstBracket = new TournamentBracket
            {
                PhaseId = firstPhase.Id,
                Matches = new List<TournamentMatch>()
            };

            int matchesInFirstRound = bracketSize / 2;
            int matchNumber = 1;

            for (int i = 0; i < matchesInFirstRound; i++)
            {
                Couple? coupleOne = i < shuffledCouples.Count ? shuffledCouples[i] : null;
                Couple? coupleTwo = (i + matchesInFirstRound) < shuffledCouples.Count ? shuffledCouples[i + matchesInFirstRound] : null;

                // Si una pareja no tiene oponente, obtiene "bye" (pasa automáticamente)
                var match = new TournamentMatch
                {
                    TournamentMatchState = (coupleOne != null && coupleTwo != null) ? "Pendiente" : "Bye",
                    Result = "",
                    CoupleOneId = coupleOne?.Id,
                    CoupleTwoId = coupleTwo?.Id,
                    CourtSlotId = null, // Se asignará posteriormente
                    BracketId = 0 // Se asignará cuando se guarde el bracket
                };

                firstBracket.Matches.Add(match);
                matchNumber++;
                totalMatches++;
            }

            _context.TournamentBrackets.Add(firstBracket);
            await _context.SaveChangesAsync();

            // Generar las rondas subsiguientes (vacías, se llenarán con los ganadores)
            for (int round = 1; round < numberOfRounds; round++)
            {
                var phase = phases[round];
                var bracket = new TournamentBracket
                {
                    PhaseId = phase.Id,
                    Matches = new List<TournamentMatch>()
                };

                int matchesInRound = (int)Math.Pow(2, numberOfRounds - round - 1);

                for (int i = 0; i < matchesInRound; i++)
                {
                    var match = new TournamentMatch
                    {
                        TournamentMatchState = "Pendiente",
                        Result = "",
                        CoupleOneId = null, // Se llenará con el ganador
                        CoupleTwoId = null, // Se llenará con el ganador
                        CourtSlotId = null,
                        BracketId = 0
                    };

                    bracket.Matches.Add(match);
                    totalMatches++;
                }

                _context.TournamentBrackets.Add(bracket);
            }

            await _context.SaveChangesAsync();

            // Actualizar el torneo
            tournament.CurrentPhase = phases[0].PhaseName;
            tournament.TournamentStatus = TournamentStatus.EnProgreso;
            await _context.SaveChangesAsync();

            // Preparar respuesta
            var response = new GenerateBracketResponseDto
            {
                TournamentId = tournamentId,
                Message = "Bracket generado exitosamente",
                TotalPhases = phases.Count,
                TotalMatches = totalMatches,
                Phases = await MapPhasesToDto(phases)
            };

            return response;
        }

        public async Task<TournamentPhaseWithBracketsDto?> GetTournamentBracketAsync(int tournamentId)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.TournamentPhases)
                .ThenInclude(p => p.Brackets)
                .ThenInclude(b => b.Matches)
                .ThenInclude(m => m.CoupleOne)
                .ThenInclude(c => c.Players)
                .Include(t => t.TournamentPhases)
                .ThenInclude(p => p.Brackets)
                .ThenInclude(b => b.Matches)
                .ThenInclude(m => m.CoupleTwo)
                .ThenInclude(c => c.Players)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null || !tournament.TournamentPhases.Any())
            {
                return null;
            }

            // Obtener la fase actual
            var currentPhase = tournament.TournamentPhases
                .FirstOrDefault(p => p.PhaseName == tournament.CurrentPhase);

            if (currentPhase == null)
            {
                currentPhase = tournament.TournamentPhases.OrderBy(p => p.StartDate).First();
            }

            return await MapPhaseToDto(currentPhase);
        }

        private async Task<List<TournamentPhaseWithBracketsDto>> MapPhasesToDto(List<TournamentPhase> phases)
        {
            var phasesDto = new List<TournamentPhaseWithBracketsDto>();

            foreach (var phase in phases)
            {
                phasesDto.Add(await MapPhaseToDto(phase));
            }

            return phasesDto;
        }

        private async Task<TournamentPhaseWithBracketsDto> MapPhaseToDto(TournamentPhase phase)
        {
            var phaseDto = new TournamentPhaseWithBracketsDto
            {
                Id = phase.Id,
                TournamentId = phase.TournamentId,
                PhaseName = phase.PhaseName,
                StartDate = phase.StartDate,
                EndDate = phase.EndDate,
                Brackets = new List<TournamentBracketDto>()
            };

            // Cargar brackets con sus matches y parejas
            var brackets = await _context.TournamentBrackets
                .Where(b => b.PhaseId == phase.Id)
                .Include(b => b.Matches)
                .ThenInclude(m => m.CoupleOne)
                .ThenInclude(c => c.Players)
                .Include(b => b.Matches)
                .ThenInclude(m => m.CoupleTwo)
                .ThenInclude(c => c.Players)
                .ToListAsync();

            foreach (var bracket in brackets)
            {
                var bracketDto = new TournamentBracketDto
                {
                    Id = bracket.Id,
                    PhaseId = bracket.PhaseId,
                    Matches = new List<TournamentMatchDto>()
                };

                int matchNumber = 1;
                foreach (var match in bracket.Matches)
                {
                    var matchDto = await MapMatchToDto(match, matchNumber);
                    bracketDto.Matches.Add(matchDto);
                    matchNumber++;
                }

                phaseDto.Brackets.Add(bracketDto);
            }

            return phaseDto;
        }

        private async Task<TournamentMatchDto> MapMatchToDto(TournamentMatch match, int matchNumber)
        {
            var matchDto = new TournamentMatchDto
            {
                Id = match.Id,
                TournamentMatchState = match.TournamentMatchState,
                Result = match.Result,
                CoupleOneId = match.CoupleOneId,
                CoupleTwoId = match.CoupleTwoId,
                CourtSlotId = match.CourtSlotId,
                MatchNumber = matchNumber
            };

            // Cargar información de programación si existe
            if (match.CourtSlotId.HasValue)
            {
                var courtSlot = await _context.CourtSlots
                    .Include(cs => cs.Court)
                    .FirstOrDefaultAsync(cs => cs.Id == match.CourtSlotId.Value);

                if (courtSlot != null)
                {
                    matchDto.ScheduledDate = courtSlot.Date;
                    matchDto.ScheduledStartTime = courtSlot.StartTime.ToTimeSpan();
                    matchDto.ScheduledEndTime = courtSlot.EndTime.ToTimeSpan();
                    matchDto.CourtId = courtSlot.CourtId;
                    matchDto.CourtName = courtSlot.Court?.Name ?? "";
                }
            }

            if (match.CoupleOneId.HasValue && match.CoupleOne != null)
            {
                matchDto.CoupleOne = await MapCoupleToDto(match.CoupleOne);
            }

            if (match.CoupleTwoId.HasValue && match.CoupleTwo != null)
            {
                matchDto.CoupleTwo = await MapCoupleToDto(match.CoupleTwo);
            }

            return matchDto;
        }

        private async Task<CoupleResponseDto> MapCoupleToDto(Couple couple)
        {
            var playerIds = couple.Players.Select(p => p.Id).ToList();

            var users = await _context.Users
                .Include(u => u.Person)
                .Where(u => u.Person != null && playerIds.Contains(u.Person.Id))
                .ToListAsync();

            var playersDto = couple.Players.Select(player =>
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

            return new CoupleResponseDto
            {
                Id = couple.Id,
                Name = couple.Name,
                Players = playersDto
            };
        }

        private int GetNextPowerOfTwo(int number)
        {
            if (number <= 0) return 1;
            
            int power = 1;
            while (power < number)
            {
                power *= 2;
            }
            return power;
        }

        private string GetPhaseName(int round)
        {
            return round switch
            {
                1 => "Final",
                2 => "Semifinales",
                3 => "Cuartos de Final",
                4 => "Octavos de Final",
                5 => "Dieciseisavos de Final",
                _ => $"Ronda {round}"
            };
        }
    }
}

