using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Tournament;
using padelya_api.DTOs.Annual;
using padelya_api.Models.Tournament;
using padelya_api.Models.Annual;
using padelya_api.Services.Annual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace padelya_api.Services
{
    public class MatchResultService : IMatchResultService
    {
        private readonly PadelYaDbContext _context;
        private readonly IAnnualTableService _annualTableService;

        public MatchResultService(PadelYaDbContext context, IAnnualTableService annualTableService)
        {
            _context = context;
            _annualTableService = annualTableService;
        }

        public async Task<MatchResultResponseDto> RegisterMatchResultAsync(RegisterMatchResultDto resultDto)
        {
            var match = await _context.TournamentMatches
                .Include(m => m.CoupleOne)
                    .ThenInclude(c => c!.Players)
                .Include(m => m.CoupleTwo)
                    .ThenInclude(c => c!.Players)
                .Include(m => m.Bracket)
                    .ThenInclude(b => b.Phase)
                        .ThenInclude(p => p.Tournament)
                .Include(m => m.CourtSlot)
                .FirstOrDefaultAsync(m => m.Id == resultDto.MatchId);

            if (match == null)
            {
                throw new ArgumentException($"No se encontró el partido con ID {resultDto.MatchId}");
            }

            if (!match.CoupleOneId.HasValue || !match.CoupleTwoId.HasValue)
            {
                throw new ArgumentException("El partido debe tener ambas parejas asignadas antes de registrar un resultado");
            }

            if (resultDto.WinnerCoupleId != match.CoupleOneId && resultDto.WinnerCoupleId != match.CoupleTwoId)
            {
                throw new ArgumentException("La pareja ganadora debe ser una de las dos parejas del partido");
            }

            if (match.TournamentMatchState == "Completed")
            {
                throw new ArgumentException("El partido ya tiene un resultado registrado. Use la opción de editar resultado.");
            }

            ValidateResultFormat(resultDto.Result, resultDto.WinnerCoupleId, match.CoupleOneId.Value, match.CoupleTwoId.Value);

            match.Result = resultDto.Result;
            match.WinnerCoupleId = resultDto.WinnerCoupleId;
            match.TournamentMatchState = "Completed";

            // Liberar el slot de la cancha si estaba programado
            if (match.CourtSlotId.HasValue && match.CourtSlot != null)
            {
                _context.CourtSlots.Remove(match.CourtSlot);
                match.CourtSlotId = null;
            }

            await _context.SaveChangesAsync();

            bool advancedToNextRound = await AdvanceWinnerToNextRound(match);

            var winnerCouple = resultDto.WinnerCoupleId == match.CoupleOneId 
                ? match.CoupleOne 
                : match.CoupleTwo;

            var winnerCoupleName = GetCoupleName(winnerCouple!);

            return new MatchResultResponseDto
            {
                MatchId = match.Id,
                WinnerCoupleId = resultDto.WinnerCoupleId,
                WinnerCoupleName = winnerCoupleName,
                Result = resultDto.Result,
                AdvancedToNextRound = advancedToNextRound,
                Message = advancedToNextRound 
                    ? $"Resultado registrado exitosamente. {winnerCoupleName} avanza a la siguiente ronda." 
                    : "Resultado registrado exitosamente."
            };
        }

        public async Task<bool> UpdateMatchResultAsync(int matchId, RegisterMatchResultDto resultDto)
        {
            var match = await _context.TournamentMatches
                .Include(m => m.CoupleOne)
                    .ThenInclude(c => c!.Players)
                .Include(m => m.CoupleTwo)
                    .ThenInclude(c => c!.Players)
                .Include(m => m.CourtSlot)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                return false;
            }

            if (resultDto.WinnerCoupleId != match.CoupleOneId && resultDto.WinnerCoupleId != match.CoupleTwoId)
            {
                throw new ArgumentException("La pareja ganadora debe ser una de las dos parejas del partido");
            }

            ValidateResultFormat(resultDto.Result, resultDto.WinnerCoupleId, match.CoupleOneId!.Value, match.CoupleTwoId!.Value);

            match.Result = resultDto.Result;
            match.WinnerCoupleId = resultDto.WinnerCoupleId;
            match.TournamentMatchState = "Completed";

            // Liberar el slot de la cancha si estaba programado
            if (match.CourtSlotId.HasValue && match.CourtSlot != null)
            {
                _context.CourtSlots.Remove(match.CourtSlot);
                match.CourtSlotId = null;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        private void ValidateResultFormat(string result, int winnerCoupleId, int coupleOneId, int coupleTwoId)
        {
            var sets = result.Split(',').Select(s => s.Trim()).ToArray();

            if (sets.Length < 2 || sets.Length > 3)
            {
                throw new ArgumentException("El resultado debe tener 2 o 3 sets");
            }

            int coupleOneWins = 0;
            int coupleTwoWins = 0;

            foreach (var set in sets)
            {
                var scores = set.Split('-');
                if (scores.Length != 2)
                {
                    throw new ArgumentException($"Formato de set inválido: {set}");
                }

                if (!int.TryParse(scores[0], out int score1) || !int.TryParse(scores[1], out int score2))
                {
                    throw new ArgumentException($"Los puntajes deben ser números: {set}");
                }

                if (score1 < 0 || score2 < 0 || score1 > 7 || score2 > 7)
                {
                    throw new ArgumentException($"Puntajes inválidos en el set: {set}");
                }

                if (score1 > score2)
                {
                    coupleOneWins++;
                }
                else if (score2 > score1)
                {
                    coupleTwoWins++;
                }
            }

            int expectedWinnerWins = sets.Length == 2 ? 2 : 2;
            int winnerSetWins = winnerCoupleId == coupleOneId ? coupleOneWins : coupleTwoWins;

            if (winnerSetWins < expectedWinnerWins)
            {
                throw new ArgumentException($"El ganador declarado no coincide con los sets ganados. Verificar resultado.");
            }
        }

        private async Task<bool> AdvanceWinnerToNextRound(TournamentMatch completedMatch)
        {
            var bracket = await _context.TournamentBrackets
                .Include(b => b.Phase)
                .ThenInclude(p => p.Tournament)
                .Include(b => b.Matches)
                .FirstOrDefaultAsync(b => b.Id == completedMatch.BracketId);

            if (bracket == null)
            {
                return false;
            }

            var currentPhase = bracket.Phase;
            var tournament = currentPhase.Tournament;

            var nextPhase = await _context.TournamentPhases
                .Include(p => p.Brackets)
                .ThenInclude(b => b.Matches)
                .Where(p => p.TournamentId == tournament.Id && p.PhaseOrder == currentPhase.PhaseOrder + 1)
                .FirstOrDefaultAsync();

            if (nextPhase == null)
            {
                // Tournament is finished - award points to all participants
                tournament.TournamentStatus = TournamentStatus.Finalizado;
                tournament.CurrentPhase = "Finalizado";
                await _context.SaveChangesAsync();
                
                // Award annual table points based on tournament placements
                await AwardTournamentPointsAsync(tournament.Id);
                
                return false;
            }

            var currentPhaseMatches = await _context.TournamentMatches
                .Where(m => m.BracketId == bracket.Id)
                .OrderBy(m => m.Id)
                .ToListAsync();

            var matchIndex = currentPhaseMatches.IndexOf(completedMatch);
            if (matchIndex == -1)
            {
                return false;
            }

            var targetMatchIndex = matchIndex / 2;

            var nextPhaseBrackets = nextPhase.Brackets.ToList();
            if (!nextPhaseBrackets.Any())
            {
                return false;
            }

            var nextPhaseMatches = await _context.TournamentMatches
                .Where(m => nextPhaseBrackets.Select(b => b.Id).Contains(m.BracketId))
                .OrderBy(m => m.Id)
                .ToListAsync();

            if (targetMatchIndex >= nextPhaseMatches.Count)
            {
                return false;
            }

            var targetMatch = nextPhaseMatches[targetMatchIndex];

            if (matchIndex % 2 == 0)
            {
                targetMatch.CoupleOneId = completedMatch.WinnerCoupleId;
            }
            else
            {
                targetMatch.CoupleTwoId = completedMatch.WinnerCoupleId;
            }

            if (targetMatch.CoupleOneId.HasValue && targetMatch.CoupleTwoId.HasValue)
            {
                targetMatch.TournamentMatchState = "Listo";
            }

            await _context.SaveChangesAsync();

            var allMatchesCompleted = currentPhaseMatches.All(m => m.TournamentMatchState == "Completed");
            
            if (allMatchesCompleted)
            {
                tournament.CurrentPhase = nextPhase.PhaseName;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        private string GetCoupleName(Couple couple)
        {
            var players = couple.Players.ToList();
            if (players.Count >= 2)
            {
                return $"{players[0].Name} {players[0].Surname} / {players[1].Name} {players[1].Surname}";
            }
            else if (players.Count == 1)
            {
                return $"{players[0].Name} {players[0].Surname}";
            }
            return "Pareja sin nombre";
        }

        private async Task AwardTournamentPointsAsync(int tournamentId)
        {
            try
            {
                // Get tournament with all necessary data
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
                    return;
                }

                // Get tournament year from start date
                var tournamentYear = tournament.TournamentStartDate.Year;

                // Get tournament scoring rules for this year
                var scoringRules = await _annualTableService.GetScoringRulesAsync(tournamentYear);
                var tournamentRule = scoringRules.FirstOrDefault(r => r.Source == ScoringSource.Tournament && r.IsActive);

                if (tournamentRule == null)
                {
                    // No tournament scoring rule configured, skip
                    return;
                }

                // Parse tournament scoring configuration
                TournamentScoringConfiguration? config = null;
                if (!string.IsNullOrEmpty(tournamentRule.ConfigurationJson))
                {
                    config = JsonSerializer.Deserialize<TournamentScoringConfiguration>(tournamentRule.ConfigurationJson);
                }

                // If no configuration, use base points for all participants
                var basePoints = tournamentRule.BasePoints;

                // Get all confirmed enrollments (participants)
                var confirmedEnrollments = tournament.Enrollments
                    .Where(e => e.Status == TournamentEnrollmentStatus.Confirmed)
                    .ToList();

                if (!confirmedEnrollments.Any())
                {
                    return;
                }

                // Calculate placements based on tournament results
                var placements = CalculateTournamentPlacements(tournament);

                // Award points to each couple based on their placement
                foreach (var enrollment in confirmedEnrollments)
                {
                    var coupleId = enrollment.Couple.Id;
                    var placement = placements.ContainsKey(coupleId) ? placements[coupleId] : 999; // Default to last place

                    // Determine points based on placement
                    int points = basePoints;
                    if (config != null)
                    {
                        points = placement switch
                        {
                            1 => config.Category1ra ?? basePoints,      // Winner
                            2 => config.Category2da ?? basePoints,      // Runner-up
                            3 or 4 => config.Category3ra ?? basePoints, // Semi-finalists
                            >= 5 and <= 8 => config.Category4ta ?? basePoints, // Quarter-finalists
                            _ => config.Category5ta ?? 0                // Participants
                        };
                    }

                    // Award points to both players in the couple
                    foreach (var player in enrollment.Couple.Players)
                    {
                        var isWinner = placement == 1;
                        await _annualTableService.ApplyPointsAsync(
                            year: tournamentYear,
                            playerId: player.Id,
                            source: ScoringSource.Tournament,
                            points: points,
                            isWin: isWinner,
                            matchId: tournamentId,
                            matchType: "Tournament",
                            scoringStrategy: "TournamentScoringStrategy",
                            recordedByUserId: null,
                            metadata: $"Torneo: {tournament.Title}, Posición: {placement}"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail tournament finalization
                Console.WriteLine($"Error awarding tournament points: {ex.Message}");
            }
        }

        private Dictionary<int, int> CalculateTournamentPlacements(Tournament tournament)
        {
            var placements = new Dictionary<int, int>();

            // Get all phases ordered by phase order (descending to start from final)
            var phases = tournament.TournamentPhases
                .OrderByDescending(p => p.PhaseOrder)
                .ToList();

            int currentPlacement = 1;

            foreach (var phase in phases)
            {
                var phaseMatches = phase.Brackets
                    .SelectMany(b => b.Matches)
                    .Where(m => m.TournamentMatchState == "Completed")
                    .ToList();

                if (!phaseMatches.Any())
                {
                    continue;
                }

                // Winners advance, losers get placed
                var losers = new List<int>();

                foreach (var match in phaseMatches)
                {
                    if (match.WinnerCoupleId.HasValue)
                    {
                        var loserCoupleId = match.CoupleOneId == match.WinnerCoupleId
                            ? match.CoupleTwoId
                            : match.CoupleOneId;

                        if (loserCoupleId.HasValue && !placements.ContainsKey(loserCoupleId.Value))
                        {
                            losers.Add(loserCoupleId.Value);
                        }

                        // If this is the final match, also place the winner
                        if (phase.PhaseOrder == phases.Max(p => p.PhaseOrder))
                        {
                            if (!placements.ContainsKey(match.WinnerCoupleId.Value))
                            {
                                placements[match.WinnerCoupleId.Value] = currentPlacement++;
                            }
                        }
                    }
                }

                // Assign placement to all losers of this phase
                foreach (var loserCoupleId in losers)
                {
                    placements[loserCoupleId] = currentPlacement;
                }

                if (losers.Any())
                {
                    currentPlacement += losers.Count;
                }
            }

            return placements;
        }
    }
}

