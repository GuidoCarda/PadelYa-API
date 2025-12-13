using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Annual;
using padelya_api.Models;
using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual
{
    public class AnnualTableService : IAnnualTableService
    {
        private readonly PadelYaDbContext _context;

        public AnnualTableService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<AnnualTable> GetOrCreateForYearAsync(int year)
        {
            var table = await _context.AnnualTables
                .Include(a => a.ScoringRules)
                .FirstOrDefaultAsync(a => a.Year == year);
            if (table == null)
            {
                table = new AnnualTable { Year = year, Status = AnnualTableStatus.Active };
                _context.AnnualTables.Add(table);
                await _context.SaveChangesAsync();

                // Seed default scoring rules con configuraciones por defecto
                var defaultChallengeConfig = System.Text.Json.JsonSerializer.Serialize(new DTOs.Annual.ChallengeScoringConfiguration());
                var defaultTournamentConfig = System.Text.Json.JsonSerializer.Serialize(new DTOs.Annual.TournamentScoringConfiguration());
                
                var defaultRules = new List<ScoringRule>
                {
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.Challenge, BasePoints = 20, Multiplier = 1.0f, MaxPoints = null, IsActive = true, ConfigurationJson = defaultChallengeConfig },
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.Tournament, BasePoints = 50, Multiplier = 1.0f, MaxPoints = null, IsActive = true, ConfigurationJson = defaultTournamentConfig },
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.Class, BasePoints = 5, Multiplier = 1.0f, MaxPoints = 10, IsActive = true },
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.MatchWin, BasePoints = 10, Multiplier = 1.0f, MaxPoints = null, IsActive = true },
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.MatchLoss, BasePoints = 2, Multiplier = 1.0f, MaxPoints = null, IsActive = true },
                };
                await _context.ScoringRules.AddRangeAsync(defaultRules);

                // Crear entries vacías solo para jugadores que realmente existen en el sistema
                var playerIds = await _context.Players.Select(p => p.Id).ToListAsync();
                foreach (var pid in playerIds)
                {
                    var entry = new RankingEntry
                    {
                        AnnualTableId = table.Id,
                        PlayerId = pid,
                        PointsTotal = 0,
                        PointsFromChallenges = 0,
                        PointsFromTournaments = 0,
                        PointsFromClasses = 0,
                        PointsFromMatchWins = 0,
                        PointsFromMatchLosses = 0,
                        Wins = 0,
                        Losses = 0,
                        Draws = 0
                    };
                    _context.RankingEntries.Add(entry);
                }

                await _context.SaveChangesAsync();
            }
            return table;
        }

        public async Task<List<RankingEntry>> GetRankingAsync(int year, int? playerId = null, string? from = null, string? to = null)
        {
            await GetOrCreateForYearAsync(year);

            // Obtener solo jugadores que realmente existen en el sistema
            var validPlayerIds = await _context.Players.Select(p => p.Id).ToListAsync();

            var query = _context.RankingEntries
                .Where(e => e.AnnualTable.Year == year && validPlayerIds.Contains(e.PlayerId))
                .AsQueryable();

            if (playerId.HasValue)
            {
                query = query.Where(e => e.PlayerId == playerId.Value);
            }

            // from/to placeholders for future date filters

            return await query
                .OrderByDescending(e => e.PointsTotal)
                .ToListAsync();
        }

        public async Task<List<RankingEntryDto>> GetRankingWithNamesAsync(int year, int? playerId = null, string? from = null, string? to = null)
        {
            var entries = await GetRankingAsync(year, playerId, from, to);
            var playerIds = entries.Select(e => e.PlayerId).Distinct().ToList();
            
            var users = await _context.Users
                .Where(u => u.PersonId.HasValue && playerIds.Contains(u.PersonId.Value))
                .ToListAsync();

            var result = new List<RankingEntryDto>();
            int position = 1;
            
            foreach (var entry in entries)
            {
                var user = users.FirstOrDefault(u => u.PersonId == entry.PlayerId);
                result.Add(new RankingEntryDto
                {
                    Id = entry.Id,
                    PlayerId = entry.PlayerId,
                    PlayerName = user?.Name,
                    PlayerSurname = user?.Surname,
                    Position = position++,
                    PointsTotal = entry.PointsTotal,
                    Wins = entry.Wins,
                    Losses = entry.Losses,
                    Draws = entry.Draws,
                    PointsFromTournaments = entry.PointsFromTournaments,
                    PointsFromChallenges = entry.PointsFromChallenges,
                    PointsFromClasses = entry.PointsFromClasses,
                    PointsFromMatchWins = entry.PointsFromMatchWins,
                    PointsFromMatchLosses = entry.PointsFromMatchLosses,
                    LastUpdatedAt = entry.LastUpdatedAt
                });
            }

            return result;
        }

        public async Task<AnnualTableStatisticsDto> GetStatisticsAsync(int year)
        {
            var table = await GetOrCreateForYearAsync(year);
            var ranking = await GetRankingAsync(year);

            var challenges = await _context.Set<Models.Challenge.Challenge>()
                .Where(c => c.Year == year)
                .ToListAsync();

            var statistics = new AnnualTableStatisticsDto
            {
                TotalPlayers = ranking.Count,
                ActivePlayers = ranking.Count(r => r.PointsTotal > 0),
                TotalChallenges = challenges.Count,
                PendingChallenges = challenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Pending),
                TotalPointsAwarded = ranking.Sum(r => r.PointsTotal),
                PointsBySource = new Dictionary<string, int>
                {
                    { "Tournaments", ranking.Sum(r => r.PointsFromTournaments) },
                    { "Challenges", ranking.Sum(r => r.PointsFromChallenges) },
                    { "Classes", ranking.Sum(r => r.PointsFromClasses) },
                    { "MatchWins", ranking.Sum(r => r.PointsFromMatchWins) },
                    { "MatchLosses", ranking.Sum(r => r.PointsFromMatchLosses) }
                }
            };

            return statistics;
        }

        public async Task<AnnualTableStatus> GetStatusAsync(int year)
        {
            var table = await GetOrCreateForYearAsync(year);
            return table.Status;
        }

        public async Task<AnnualTable> UpdateStatusAsync(int year, AnnualTableStatus status)
        {
            var table = await GetOrCreateForYearAsync(year);
            table.Status = status;
            if (status == AnnualTableStatus.Suspended)
            {
                table.SuspendedAt = DateTime.UtcNow;
            }
            if (status == AnnualTableStatus.Active)
            {
                table.ResumedAt = DateTime.UtcNow;
            }
            if (status == AnnualTableStatus.Closed)
            {
                table.ClosedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return table;
        }

        public async Task<List<ScoringRuleDto>> GetScoringRulesAsync(int year)
        {
            var table = await GetOrCreateForYearAsync(year);
            var rules = await _context.ScoringRules
                .Where(r => r.AnnualTableId == table.Id)
                .Select(r => new ScoringRuleDto
                {
                    Id = r.Id,
                    AnnualTableId = r.AnnualTableId,
                    Source = r.Source,
                    BasePoints = r.BasePoints,
                    Multiplier = r.Multiplier,
                    MaxPoints = r.MaxPoints,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    DeactivatedAt = r.DeactivatedAt,
                    ConfigurationJson = r.ConfigurationJson
                })
                .ToListAsync();
            return rules;
        }

        public async Task<List<ScoringRuleDto>> UpsertScoringRulesAsync(int year, List<ScoringRuleDto> rules)
        {
            var table = await GetOrCreateForYearAsync(year);

            var existing = await _context.ScoringRules
                .Where(r => r.AnnualTableId == table.Id)
                .ToListAsync();

            _context.ScoringRules.RemoveRange(existing);
            
            var newRules = rules.Select(r => new ScoringRule
            {
                Id = 0,
                AnnualTableId = table.Id,
                Source = r.Source,
                BasePoints = r.BasePoints,
                Multiplier = r.Multiplier,
                MaxPoints = r.MaxPoints,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                DeactivatedAt = r.DeactivatedAt,
                ConfigurationJson = r.ConfigurationJson
            }).ToList();
            
            await _context.ScoringRules.AddRangeAsync(newRules);
            await _context.SaveChangesAsync();
            
            // Recargar las reglas guardadas para obtener los IDs correctos
            var savedRules = await _context.ScoringRules
                .Where(nr => nr.AnnualTableId == table.Id)
                .OrderBy(nr => nr.Id)
                .ToListAsync();

            return savedRules.Select(r => new ScoringRuleDto
            {
                Id = r.Id,
                AnnualTableId = r.AnnualTableId,
                Source = r.Source,
                BasePoints = r.BasePoints,
                Multiplier = r.Multiplier,
                MaxPoints = r.MaxPoints,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                DeactivatedAt = r.DeactivatedAt,
                ConfigurationJson = r.ConfigurationJson
            }).ToList();
        }

        public async Task ApplyPointsAsync(int year, int playerId, ScoringSource source, int points, bool isWin, int? matchId = null, string? matchType = null, string? scoringStrategy = null, int? recordedByUserId = null, string? metadata = null)
        {
            var table = await GetOrCreateForYearAsync(year);

            var entry = await _context.RankingEntries
                .FirstOrDefaultAsync(e => e.AnnualTableId == table.Id && e.PlayerId == playerId);

            if (entry == null)
            {
                entry = new RankingEntry
                {
                    AnnualTableId = table.Id,
                    PlayerId = playerId
                };
                _context.RankingEntries.Add(entry);
                await _context.SaveChangesAsync(); // Guardar para obtener el ID
            }

            entry.PointsTotal += points;
            entry.LastUpdatedAt = DateTime.UtcNow;

            switch (source)
            {
                case ScoringSource.Tournament:
                    entry.PointsFromTournaments += points;
                    break;
                case ScoringSource.Challenge:
                    entry.PointsFromChallenges += points;
                    if (isWin) entry.Wins++; else entry.Losses++;
                    break;
                case ScoringSource.Class:
                    entry.PointsFromClasses += points;
                    break;
                case ScoringSource.MatchWin:
                    entry.PointsFromMatchWins += points;
                    entry.Wins++;
                    break;
                case ScoringSource.MatchLoss:
                    entry.PointsFromMatchLosses += points;
                    entry.Losses++;
                    break;
            }

            // Registrar trazabilidad
            var trace = new RankingTrace
            {
                MatchId = matchId,
                MatchType = matchType ?? source.ToString(),
                Points = points,
                AnnualTableId = table.Id,
                RankingEntryId = entry.Id,
                PlayerId = playerId,
                Source = source,
                ScoringStrategy = scoringStrategy ?? GetStrategyName(source),
                IsWin = isWin,
                RecordedAt = DateTime.UtcNow,
                RecordedByUserId = recordedByUserId,
                Metadata = metadata
            };

            _context.RankingTraces.Add(trace);
            await _context.SaveChangesAsync();
        }

        private string GetStrategyName(ScoringSource source)
        {
            return source switch
            {
                ScoringSource.Challenge => "ChallengeScoringStrategy",
                ScoringSource.Tournament => "TournamentScoringStrategy",
                ScoringSource.Class => "ClassScoringStrategy",
                ScoringSource.MatchWin => "MatchWinScoringStrategy",
                ScoringSource.MatchLoss => "MatchLossScoringStrategy",
                _ => "UnknownStrategy"
            };
        }
    }
}

