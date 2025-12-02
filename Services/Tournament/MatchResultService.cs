using Microsoft.EntityFrameworkCore;
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
            // Buscar el partido con sus relaciones
            var match = await _context.TournamentMatches
                .Include(m => m.CoupleOne)
                    .ThenInclude(c => c!.Players)
                .Include(m => m.CoupleTwo)
                    .ThenInclude(c => c!.Players)
                .Include(m => m.Bracket)
                    .ThenInclude(b => b.Tournament)
                .FirstOrDefaultAsync(m => m.Id == resultDto.MatchId);

            if (match == null)
            {
                throw new ArgumentException($"No se encontró el partido con ID {resultDto.MatchId}");
            }

            // Validar que el partido tenga ambas parejas asignadas
            if (!match.CoupleOneId.HasValue || !match.CoupleTwoId.HasValue)
            {
                throw new ArgumentException("El partido debe tener ambas parejas asignadas antes de registrar un resultado");
            }

            // Validar que el ganador sea una de las dos parejas del partido
            if (resultDto.WinnerCoupleId != match.CoupleOneId && resultDto.WinnerCoupleId != match.CoupleTwoId)
            {
                throw new ArgumentException("La pareja ganadora debe ser una de las dos parejas del partido");
            }

            // Validar que el partido no esté ya completado (opcional, permitir edición)
            if (match.TournamentMatchState == "Completed")
            {
                throw new ArgumentException("El partido ya tiene un resultado registrado. Use la opción de editar resultado.");
            }

            // Validar formato del resultado (básico, ya hay validación en el DTO)
            ValidateResultFormat(resultDto.Result, resultDto.WinnerCoupleId, match.CoupleOneId.Value, match.CoupleTwoId.Value);

            // Actualizar el partido con el resultado
            match.Result = resultDto.Result;
            match.WinnerCoupleId = resultDto.WinnerCoupleId;
            match.TournamentMatchState = "Completed";

            await _context.SaveChangesAsync();

            // Verificar si debe avanzar a la siguiente ronda
            bool advancedToNextRound = await AdvanceWinnerToNextRound(match);

            // Preparar respuesta
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
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                return false;
            }

            // Validar que el ganador sea una de las dos parejas
            if (resultDto.WinnerCoupleId != match.CoupleOneId && resultDto.WinnerCoupleId != match.CoupleTwoId)
            {
                throw new ArgumentException("La pareja ganadora debe ser una de las dos parejas del partido");
            }

            // Validar formato del resultado
            ValidateResultFormat(resultDto.Result, resultDto.WinnerCoupleId, match.CoupleOneId!.Value, match.CoupleTwoId!.Value);

            // Actualizar resultado
            match.Result = resultDto.Result;
            match.WinnerCoupleId = resultDto.WinnerCoupleId;
            match.TournamentMatchState = "Completed";

            await _context.SaveChangesAsync();

            return true;
        }

        private void ValidateResultFormat(string result, int winnerCoupleId, int coupleOneId, int coupleTwoId)
        {
            // Formato esperado: "6-4, 6-3" o "6-4, 4-6, 7-5"
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

                // Validar que los puntajes sean válidos para pádel
                if (score1 < 0 || score2 < 0 || score1 > 7 || score2 > 7)
                {
                    throw new ArgumentException($"Puntajes inválidos en el set: {set}");
                }

                // Determinar ganador del set
                if (score1 > score2)
                {
                    coupleOneWins++;
                }
                else if (score2 > score1)
                {
                    coupleTwoWins++;
                }
            }

            // Verificar que el ganador declarado coincida con los sets ganados
            int expectedWinnerWins = sets.Length == 2 ? 2 : 2; // Necesita 2 sets para ganar
            int winnerSetWins = winnerCoupleId == coupleOneId ? coupleOneWins : coupleTwoWins;

            if (winnerSetWins < expectedWinnerWins)
            {
                throw new ArgumentException($"El ganador declarado no coincide con los sets ganados. Verificar resultado.");
            }
        }

        private async Task<bool> AdvanceWinnerToNextRound(TournamentMatch completedMatch)
        {
            // Buscar el siguiente partido en la fase actual o siguiente fase
            // La lógica depende de cómo está estructurado tu bracket
            
            // Por ahora, buscar si hay un partido en una fase superior que esté esperando al ganador de este partido
            var bracket = completedMatch.Bracket;
            var tournament = bracket.Tournament;

            // Buscar la siguiente fase (si existe)
            var nextPhase = await _context.TournamentBrackets
                .Where(b => b.TournamentId == tournament.Id && b.PhaseOrder == bracket.PhaseOrder + 1)
                .FirstOrDefaultAsync();

            if (nextPhase == null)
            {
                // Es la final, no hay siguiente ronda
                return false;
            }

            // Buscar el partido correspondiente en la siguiente fase
            // Esta lógica puede variar según tu estructura de eliminación
            var nextPhaseMatches = await _context.TournamentMatches
                .Where(m => m.BracketId == nextPhase.Id)
                .ToListAsync();

            // Encontrar el partido que debería recibir al ganador
            // (Esto depende de la lógica de tu bracket, por ahora asignamos al primer partido sin parejas completas)
            var targetMatch = nextPhaseMatches.FirstOrDefault(m => 
                !m.CoupleOneId.HasValue || !m.CoupleTwoId.HasValue);

            if (targetMatch != null)
            {
                // Asignar ganador a la siguiente ronda
                if (!targetMatch.CoupleOneId.HasValue)
                {
                    targetMatch.CoupleOneId = completedMatch.WinnerCoupleId;
                }
                else if (!targetMatch.CoupleTwoId.HasValue)
                {
                    targetMatch.CoupleTwoId = completedMatch.WinnerCoupleId;
                }

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
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

