using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Tournament;
using padelya_api.Models.Tournament;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public class MatchResultService : IMatchResultService
    {
        private readonly PadelYaDbContext _context;

        public MatchResultService(PadelYaDbContext context)
        {
            _context = context;
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
                tournament.TournamentStatus = TournamentStatus.Finalizado;
                tournament.CurrentPhase = "Finalizado";
                await _context.SaveChangesAsync();
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
    }
}

