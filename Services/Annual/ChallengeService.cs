using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Challenge;
using padelya_api.Models.Annual;
using padelya_api.Models.Challenge;
using padelya_api.Services.Notification;
using padelya_api.Models.Notification;
using padelya_api.Models;
using padelya_api.Services.Annual.Scoring;

namespace padelya_api.Services.Annual
{
    public class ChallengeService : IChallengeService
    {
        private readonly PadelYaDbContext _context;
        private readonly IAnnualTableService _annualService;
        private readonly INotificationService _notification;
        private readonly IScoringService _scoringService;

        public ChallengeService(PadelYaDbContext context, IAnnualTableService annualService, INotificationService notification, IScoringService scoringService)
        {
            _context = context;
            _annualService = annualService;
            _notification = notification;
            _scoringService = scoringService;
        }

        public async Task<Challenge> CreateAsync(int year, CreateChallengeDto dto)
        {
            var table = await _annualService.GetOrCreateForYearAsync(year);
            if (table.Status != AnnualTableStatus.Active)
            {
                throw new InvalidOperationException("La tabla anual no está activa.");
            }

            // Validar que los jugadores existan
            var playerIds = new[] { dto.RequesterPlayerId, dto.RequesterPartnerPlayerId, dto.TargetPlayerId, dto.TargetPartnerPlayerId };
            var validPlayers = await _context.Players
                .Where(p => playerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            if (validPlayers.Count != playerIds.Length)
            {
                throw new InvalidOperationException("Uno o más jugadores no existen.");
            }

            // Validar que no se desafíe a sí mismo
            if (dto.RequesterPlayerId == dto.TargetPlayerId || 
                dto.RequesterPlayerId == dto.TargetPartnerPlayerId ||
                dto.RequesterPartnerPlayerId == dto.TargetPlayerId ||
                dto.RequesterPartnerPlayerId == dto.TargetPartnerPlayerId)
            {
                throw new InvalidOperationException("No puedes desafiar a tu propia pareja.");
            }

            var ranking = await _context.RankingEntries
                .Include(e => e.AnnualTable)
                .Where(e => e.AnnualTable.Year == year)
                .ToListAsync();

            int requesterPoints = ranking.FirstOrDefault(r => r.PlayerId == dto.RequesterPlayerId)?.PointsTotal ?? 0;
            int targetPoints = ranking.FirstOrDefault(r => r.PlayerId == dto.TargetPlayerId)?.PointsTotal ?? 0;

            if (Math.Abs(requesterPoints - targetPoints) > 100)
            {
                throw new InvalidOperationException("La diferencia de puntos entre las parejas supera los 100.");
            }

            var challenge = new Challenge
            {
                Year = year,
                RequesterPlayerId = dto.RequesterPlayerId,
                RequesterPartnerPlayerId = dto.RequesterPartnerPlayerId,
                TargetPlayerId = dto.TargetPlayerId,
                TargetPartnerPlayerId = dto.TargetPartnerPlayerId,
                RequesterPointsAtCreation = requesterPoints,
                TargetPointsAtCreation = targetPoints
            };

            _context.Add(challenge);
            await _context.SaveChangesAsync();

            // Notificar a jugadores target - buscar UserId basado en PersonId
            var targetUsers = await _context.Users
                .Where(u => u.PersonId == challenge.TargetPlayerId || u.PersonId == challenge.TargetPartnerPlayerId)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var userId in targetUsers)
            {
                await _notification.SendAsync(userId, NotificationType.ChallengeCreated, new { challengeId = challenge.Id });
            }
            return challenge;
        }

        public async Task<Challenge> RespondAsync(int id, bool accept)
        {
            var challenge = await _context.Set<Challenge>().FindAsync(id)
                ?? throw new KeyNotFoundException("Desafío no encontrado");
            if (challenge.Status != ChallengeStatus.Pending)
            {
                throw new InvalidOperationException("El desafío ya fue respondido.");
            }
            challenge.Status = accept ? ChallengeStatus.Accepted : ChallengeStatus.Rejected;
            challenge.RespondedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Notificar a solicitantes - buscar UserId basado en PersonId
            var requesterUsers = await _context.Users
                .Where(u => u.PersonId == challenge.RequesterPlayerId || u.PersonId == challenge.RequesterPartnerPlayerId)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var userId in requesterUsers)
            {
                await _notification.SendAsync(userId, NotificationType.ChallengeResponded, new { challengeId = id, accept });
            }
            return challenge;
        }

        public async Task<Challenge> RegisterResultAsync(int id, RegisterChallengeResultDto dto)
        {
            var challenge = await _context.Set<Challenge>().FindAsync(id)
                ?? throw new KeyNotFoundException("Desafío no encontrado");
            if (challenge.Status != ChallengeStatus.Accepted)
            {
                throw new InvalidOperationException("El desafío no está aceptado para registrar resultado.");
            }

            var table = await _annualService.GetOrCreateForYearAsync(challenge.Year);
            if (table.Status != AnnualTableStatus.Active)
            {
                throw new InvalidOperationException("La tabla anual no está activa.");
            }

            // Validar que el ganador sea uno de los participantes
            var isRequesterWinner = (dto.WinnerPlayerId == challenge.RequesterPlayerId && dto.WinnerPartnerPlayerId == challenge.RequesterPartnerPlayerId) ||
                                   (dto.WinnerPlayerId == challenge.RequesterPartnerPlayerId && dto.WinnerPartnerPlayerId == challenge.RequesterPlayerId);
            var isTargetWinner = (dto.WinnerPlayerId == challenge.TargetPlayerId && dto.WinnerPartnerPlayerId == challenge.TargetPartnerPlayerId) ||
                                (dto.WinnerPlayerId == challenge.TargetPartnerPlayerId && dto.WinnerPartnerPlayerId == challenge.TargetPlayerId);

            if (!isRequesterWinner && !isTargetWinner)
            {
                throw new InvalidOperationException("El ganador debe ser una de las parejas participantes del desafío.");
            }

            // Validar formato de sets (básico)
            if (string.IsNullOrWhiteSpace(dto.Sets))
            {
                throw new InvalidOperationException("El resultado de los sets es requerido.");
            }

            challenge.Sets = dto.Sets;
            challenge.WinnerPlayerId = dto.WinnerPlayerId;
            challenge.WinnerPartnerPlayerId = dto.WinnerPartnerPlayerId;
            challenge.PlayedAt = DateTime.UtcNow;
            challenge.Status = ChallengeStatus.Played;

            // Obtener reglas de challenge
            var rule = await _context.ScoringRules
                .Include(r => r.AnnualTable)
                .Where(r => r.AnnualTableId == table.Id && r.Source == ScoringSource.Challenge && r.IsActive)
                .FirstOrDefaultAsync();

            // Obtener ranking actualizado para calcular posiciones
            var ranking = await _annualService.GetRankingAsync(challenge.Year);
            var rankingOrdered = ranking
                .OrderByDescending(r => r.PointsTotal)
                .ToList();
            
            var rankingWithPositions = rankingOrdered
                .Select((r, index) => new { PlayerId = r.PlayerId, Position = index + 1 })
                .ToList();

            // Determinar quién es el rival (el perdedor) - reutilizar isRequesterWinner ya calculado arriba
            var rivalPlayerId = isRequesterWinner ? challenge.TargetPlayerId : challenge.RequesterPlayerId;
            var winnerPlayerId = isRequesterWinner ? challenge.RequesterPlayerId : challenge.TargetPlayerId;

            // Obtener posiciones
            var rivalPosition = rankingWithPositions.FirstOrDefault(r => r.PlayerId == rivalPlayerId)?.Position ?? 999;
            var winnerPosition = rankingWithPositions.FirstOrDefault(r => r.PlayerId == winnerPlayerId)?.Position ?? 999;

            // Calcular puntos usando Strategy con contexto
            var scoringContext = new ChallengeScoringContext
            {
                RivalPosition = rivalPosition,
                WinnerPosition = winnerPosition
            };

            var points = rule != null 
                ? _scoringService.ComputePoints(ScoringSource.Challenge, rule, scoringContext)
                : 0;
            
            challenge.PointsAwardedPerPlayer = points;

            // Sumar puntos a ambos jugadores ganadores con trazabilidad
            await _annualService.ApplyPointsAsync(
                challenge.Year, 
                dto.WinnerPlayerId, 
                ScoringSource.Challenge, 
                points, 
                true,
                matchId: challenge.Id,
                matchType: "Challenge",
                scoringStrategy: "ChallengeScoringStrategy",
                metadata: System.Text.Json.JsonSerializer.Serialize(new { sets = dto.Sets, challengeId = challenge.Id })
            );
            await _annualService.ApplyPointsAsync(
                challenge.Year, 
                dto.WinnerPartnerPlayerId, 
                ScoringSource.Challenge, 
                points, 
                true,
                matchId: challenge.Id,
                matchType: "Challenge",
                scoringStrategy: "ChallengeScoringStrategy",
                metadata: System.Text.Json.JsonSerializer.Serialize(new { sets = dto.Sets, challengeId = challenge.Id })
            );

            await _context.SaveChangesAsync();

            // Notificar cambio de ranking a ambos ganadores - buscar UserId basado en PersonId
            var winnerUsers = await _context.Users
                .Where(u => u.PersonId == dto.WinnerPlayerId || u.PersonId == dto.WinnerPartnerPlayerId)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var userId in winnerUsers)
            {
                await _notification.SendAsync(userId, NotificationType.RankingChanged, new { challengeId = id, points });
            }
            return challenge;
        }

        public async Task<List<Challenge>> GetHistoryAsync(int playerId, int? year = null)
        {
            var q = _context.Set<Challenge>().AsQueryable();
            q = q.Where(c => c.RequesterPlayerId == playerId || c.RequesterPartnerPlayerId == playerId || c.TargetPlayerId == playerId || c.TargetPartnerPlayerId == playerId);
            if (year.HasValue) q = q.Where(c => c.Year == year.Value);
            return await q.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<Challenge> ValidateAsync(int id, RegisterChallengeResultDto dto, int? adminUserId = null)
        {
            var challenge = await _context.Set<Challenge>().FindAsync(id)
                ?? throw new KeyNotFoundException("Desafío no encontrado");

            // Permitir revalidación por admin y corrección
            var table = await _annualService.GetOrCreateForYearAsync(challenge.Year);
            if (table.Status != AnnualTableStatus.Active)
            {
                throw new InvalidOperationException("La tabla anual no está activa.");
            }

            // Validar que el ganador sea uno de los participantes
            var isRequesterWinner = (dto.WinnerPlayerId == challenge.RequesterPlayerId && dto.WinnerPartnerPlayerId == challenge.RequesterPartnerPlayerId) ||
                                   (dto.WinnerPlayerId == challenge.RequesterPartnerPlayerId && dto.WinnerPartnerPlayerId == challenge.RequesterPlayerId);
            var isTargetWinner = (dto.WinnerPlayerId == challenge.TargetPlayerId && dto.WinnerPartnerPlayerId == challenge.TargetPartnerPlayerId) ||
                                (dto.WinnerPlayerId == challenge.TargetPartnerPlayerId && dto.WinnerPartnerPlayerId == challenge.TargetPlayerId);

            if (!isRequesterWinner && !isTargetWinner)
            {
                throw new InvalidOperationException("El ganador debe ser una de las parejas participantes del desafío.");
            }

            // Validar formato de sets (básico)
            if (string.IsNullOrWhiteSpace(dto.Sets))
            {
                throw new InvalidOperationException("El resultado de los sets es requerido.");
            }

            challenge.Sets = dto.Sets;
            challenge.WinnerPlayerId = dto.WinnerPlayerId;
            challenge.WinnerPartnerPlayerId = dto.WinnerPartnerPlayerId;
            challenge.Status = ChallengeStatus.Played;
            challenge.ValidatedAt = DateTime.UtcNow;
            if (adminUserId.HasValue)
            {
                challenge.ValidatedByAdminUserId = adminUserId.Value;
            }

            var rule = await _context.ScoringRules
                .Include(r => r.AnnualTable)
                .Where(r => r.AnnualTableId == table.Id && r.Source == ScoringSource.Challenge && r.IsActive)
                .FirstOrDefaultAsync();

            // Obtener ranking actualizado para calcular posiciones
            var ranking = await _annualService.GetRankingAsync(challenge.Year);
            var rankingOrdered = ranking
                .OrderByDescending(r => r.PointsTotal)
                .ToList();
            
            var rankingWithPositions = rankingOrdered
                .Select((r, index) => new { PlayerId = r.PlayerId, Position = index + 1 })
                .ToList();

            // Determinar quién es el rival (el perdedor) - reutilizar isRequesterWinner ya calculado arriba
            var rivalPlayerId = isRequesterWinner ? challenge.TargetPlayerId : challenge.RequesterPlayerId;
            var winnerPlayerId = isRequesterWinner ? challenge.RequesterPlayerId : challenge.TargetPlayerId;

            // Obtener posiciones
            var rivalPosition = rankingWithPositions.FirstOrDefault(r => r.PlayerId == rivalPlayerId)?.Position ?? 999;
            var winnerPosition = rankingWithPositions.FirstOrDefault(r => r.PlayerId == winnerPlayerId)?.Position ?? 999;

            // Calcular puntos usando Strategy con contexto
            var scoringContext = new ChallengeScoringContext
            {
                RivalPosition = rivalPosition,
                WinnerPosition = winnerPosition
            };

            var points = rule != null 
                ? _scoringService.ComputePoints(ScoringSource.Challenge, rule, scoringContext)
                : 0;
            
            challenge.PointsAwardedPerPlayer = points;

            // Aplicar puntos con trazabilidad (validación por admin)
            await _annualService.ApplyPointsAsync(
                challenge.Year, 
                dto.WinnerPlayerId, 
                ScoringSource.Challenge, 
                points, 
                true,
                matchId: challenge.Id,
                matchType: "Challenge",
                scoringStrategy: "ChallengeScoringStrategy",
                recordedByUserId: adminUserId,
                metadata: System.Text.Json.JsonSerializer.Serialize(new { sets = dto.Sets, challengeId = challenge.Id, validatedBy = adminUserId })
            );
            await _annualService.ApplyPointsAsync(
                challenge.Year, 
                dto.WinnerPartnerPlayerId, 
                ScoringSource.Challenge, 
                points, 
                true,
                matchId: challenge.Id,
                matchType: "Challenge",
                scoringStrategy: "ChallengeScoringStrategy",
                recordedByUserId: adminUserId,
                metadata: System.Text.Json.JsonSerializer.Serialize(new { sets = dto.Sets, challengeId = challenge.Id, validatedBy = adminUserId })
            );

            await _context.SaveChangesAsync();
            return challenge;
        }

        private async Task<ChallengeDto> MapChallengeToDtoAsync(Challenge challenge)
        {
            var playerIds = new List<int>
            {
                challenge.RequesterPlayerId,
                challenge.RequesterPartnerPlayerId,
                challenge.TargetPlayerId,
                challenge.TargetPartnerPlayerId
            };

            if (challenge.WinnerPlayerId.HasValue)
                playerIds.Add(challenge.WinnerPlayerId.Value);
            if (challenge.WinnerPartnerPlayerId.HasValue)
                playerIds.Add(challenge.WinnerPartnerPlayerId.Value);

            var users = await _context.Users
                .Where(u => u.PersonId.HasValue && playerIds.Contains(u.PersonId.Value))
                .ToListAsync();

            var requesterUser = users.FirstOrDefault(u => u.PersonId == challenge.RequesterPlayerId);
            var requesterPartnerUser = users.FirstOrDefault(u => u.PersonId == challenge.RequesterPartnerPlayerId);
            var targetUser = users.FirstOrDefault(u => u.PersonId == challenge.TargetPlayerId);
            var targetPartnerUser = users.FirstOrDefault(u => u.PersonId == challenge.TargetPartnerPlayerId);
            var winnerUser = challenge.WinnerPlayerId.HasValue 
                ? users.FirstOrDefault(u => u.PersonId == challenge.WinnerPlayerId.Value) 
                : null;
            var winnerPartnerUser = challenge.WinnerPartnerPlayerId.HasValue 
                ? users.FirstOrDefault(u => u.PersonId == challenge.WinnerPartnerPlayerId.Value) 
                : null;

            return new ChallengeDto
            {
                Id = challenge.Id,
                Year = challenge.Year,
                RequesterPlayerId = challenge.RequesterPlayerId,
                RequesterPlayerName = requesterUser?.Name,
                RequesterPlayerSurname = requesterUser?.Surname,
                RequesterPartnerPlayerId = challenge.RequesterPartnerPlayerId,
                RequesterPartnerName = requesterPartnerUser?.Name,
                RequesterPartnerSurname = requesterPartnerUser?.Surname,
                TargetPlayerId = challenge.TargetPlayerId,
                TargetPlayerName = targetUser?.Name,
                TargetPlayerSurname = targetUser?.Surname,
                TargetPartnerPlayerId = challenge.TargetPartnerPlayerId,
                TargetPartnerName = targetPartnerUser?.Name,
                TargetPartnerSurname = targetPartnerUser?.Surname,
                Status = challenge.Status,
                CreatedAt = challenge.CreatedAt,
                RespondedAt = challenge.RespondedAt,
                PlayedAt = challenge.PlayedAt,
                WinnerPlayerId = challenge.WinnerPlayerId,
                WinnerPlayerName = winnerUser?.Name,
                WinnerPartnerPlayerId = challenge.WinnerPartnerPlayerId,
                WinnerPartnerName = winnerPartnerUser?.Name,
                Sets = challenge.Sets,
                RequesterPointsAtCreation = challenge.RequesterPointsAtCreation,
                TargetPointsAtCreation = challenge.TargetPointsAtCreation,
                PointsAwardedPerPlayer = challenge.PointsAwardedPerPlayer,
                ValidatedByAdminUserId = challenge.ValidatedByAdminUserId,
                ValidatedAt = challenge.ValidatedAt,
                RequiresValidation = challenge.Status == ChallengeStatus.Played && !challenge.ValidatedAt.HasValue
            };
        }

        public async Task<ChallengeDto> CreateWithDetailsAsync(int year, CreateChallengeDto dto)
        {
            var challenge = await CreateAsync(year, dto);
            return await MapChallengeToDtoAsync(challenge);
        }

        public async Task<ChallengeDto> RespondWithDetailsAsync(int id, bool accept)
        {
            var challenge = await RespondAsync(id, accept);
            return await MapChallengeToDtoAsync(challenge);
        }

        public async Task<ChallengeDto> RegisterResultWithDetailsAsync(int id, RegisterChallengeResultDto dto)
        {
            var challenge = await RegisterResultAsync(id, dto);
            return await MapChallengeToDtoAsync(challenge);
        }

        public async Task<List<ChallengeDto>> GetHistoryWithDetailsAsync(int playerId, int? year = null)
        {
            var challenges = await GetHistoryAsync(playerId, year);
            var result = new List<ChallengeDto>();
            foreach (var challenge in challenges)
            {
                result.Add(await MapChallengeToDtoAsync(challenge));
            }
            return result;
        }

        public async Task<List<ChallengeDto>> GetPendingChallengesAsync(int playerId)
        {
            var challenges = await _context.Set<Challenge>()
                .Where(c => (c.TargetPlayerId == playerId || c.TargetPartnerPlayerId == playerId) 
                    && c.Status == ChallengeStatus.Pending)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var result = new List<ChallengeDto>();
            foreach (var challenge in challenges)
            {
                result.Add(await MapChallengeToDtoAsync(challenge));
            }
            return result;
        }

        public async Task<ChallengeDto> ValidateWithDetailsAsync(int id, RegisterChallengeResultDto dto, int? adminUserId = null)
        {
            var challenge = await ValidateAsync(id, dto, adminUserId);
            return await MapChallengeToDtoAsync(challenge);
        }

        public async Task<List<ChallengeDto>> GetChallengesRequiringValidationAsync()
        {
            var challenges = await _context.Set<Challenge>()
                .Where(c => c.Status == ChallengeStatus.Played && !c.ValidatedAt.HasValue)
                .OrderByDescending(c => c.PlayedAt)
                .ToListAsync();

            var result = new List<ChallengeDto>();
            foreach (var challenge in challenges)
            {
                result.Add(await MapChallengeToDtoAsync(challenge));
            }
            return result;
        }

        public async Task<List<ChallengeDto>> GetAllChallengesAsync(int? year = null)
        {
            var query = _context.Set<Challenge>().AsQueryable();
            
            if (year.HasValue)
            {
                query = query.Where(c => c.Year == year.Value);
            }

            var challenges = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var result = new List<ChallengeDto>();
            foreach (var challenge in challenges)
            {
                result.Add(await MapChallengeToDtoAsync(challenge));
            }
            return result;
        }
    }
}

