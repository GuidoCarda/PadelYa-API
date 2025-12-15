using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Annual;
using padelya_api.Models;
using padelya_api.Models.Annual;
using padelya_api.Models.Challenge;

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

            var totalPointsFromChallenges = ranking.Sum(r => r.PointsFromChallenges);
            var totalPointsFromTournaments = ranking.Sum(r => r.PointsFromTournaments);
            var totalPointsFromClasses = ranking.Sum(r => r.PointsFromClasses);
            var totalPointsAwarded = ranking.Sum(r => r.PointsTotal);
            var activePlayers = ranking.Count(r => r.PointsTotal > 0);
            var completedChallenges = challenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Played);
            var acceptedChallenges = challenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Accepted);
            var rejectedChallenges = challenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Rejected);

            var statistics = new AnnualTableStatisticsDto
            {
                TotalPlayers = ranking.Count,
                ActivePlayers = activePlayers,
                TotalChallenges = challenges.Count,
                CompletedChallenges = completedChallenges,
                PendingChallenges = challenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Pending),
                AcceptedChallenges = acceptedChallenges,
                RejectedChallenges = rejectedChallenges,
                TotalPointsAwarded = totalPointsAwarded,
                TotalPointsFromChallenges = totalPointsFromChallenges,
                TotalPointsFromTournaments = totalPointsFromTournaments,
                TotalPointsFromClasses = totalPointsFromClasses,
                AveragePointsPerPlayer = activePlayers > 0 ? (double)totalPointsAwarded / activePlayers : 0,
                ChallengeAcceptanceRate = challenges.Count > 0 ? (double)acceptedChallenges / challenges.Count * 100 : 0,
                AveragePointsPerChallenge = completedChallenges > 0 ? (double)totalPointsFromChallenges / completedChallenges : 0,
                PointsBySource = new Dictionary<string, int>
                {
                    { "Tournaments", totalPointsFromTournaments },
                    { "Challenges", totalPointsFromChallenges },
                    { "Classes", totalPointsFromClasses },
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

        public async Task<AnnualTableReportDto> GetAnnualTableReportAsync(DateTime startDate, DateTime endDate)
        {
            // Obtener todos los años que tienen actividad en el rango de fechas
            var challenges = await _context.Set<Models.Challenge.Challenge>()
                .Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate)
                .ToListAsync();

            var years = challenges.Select(c => c.Year).Distinct().ToList();
            if (!years.Any())
            {
                // Si no hay desafíos, buscar por años de tablas anuales
                years = await _context.AnnualTables
                    .Where(a => a.CreatedAt >= startDate)
                    .Select(a => a.Year)
                    .Distinct()
                    .ToListAsync();
            }

            if (!years.Any())
            {
                // Si aún no hay años, usar el año actual
                years = new List<int> { DateTime.UtcNow.Year };
            }

            var allRankingEntries = new List<RankingEntry>();
            var allChallenges = new List<Models.Challenge.Challenge>();

            foreach (var year in years)
            {
                var ranking = await GetRankingAsync(year);
                allRankingEntries.AddRange(ranking);

                var yearChallenges = await _context.Set<Models.Challenge.Challenge>()
                    .Where(c => c.Year == year && 
                               (c.CreatedAt >= startDate && c.CreatedAt <= endDate ||
                                c.PlayedAt >= startDate && c.PlayedAt <= endDate ||
                                c.RespondedAt >= startDate && c.RespondedAt <= endDate))
                    .ToListAsync();
                allChallenges.AddRange(yearChallenges);
            }

            // Obtener información de jugadores
            var playerIds = allRankingEntries.Select(e => e.PlayerId)
                .Union(allChallenges.SelectMany(c => new[] { c.RequesterPlayerId, c.RequesterPartnerPlayerId, c.TargetPlayerId, c.TargetPartnerPlayerId }))
                .Distinct()
                .ToList();

            var users = await _context.Users
                .Where(u => u.PersonId.HasValue && playerIds.Contains(u.PersonId.Value))
                .ToListAsync();

            var playerDict = users.ToDictionary(u => u.PersonId!.Value, u => new { u.Name, u.Surname });

            // Estadísticas generales
            var totalPlayers = allRankingEntries.Select(e => e.PlayerId).Distinct().Count();
            var activePlayers = allRankingEntries.Count(e => e.PointsTotal > 0);
            var totalChallenges = allChallenges.Count;
            var completedChallenges = allChallenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Played);
            var pendingChallenges = allChallenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Pending);
            var acceptedChallenges = allChallenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Accepted);
            var rejectedChallenges = allChallenges.Count(c => c.Status == Models.Challenge.ChallengeStatus.Rejected);

            var totalPointsAwarded = allRankingEntries.Sum(e => e.PointsTotal);
            var totalPointsFromChallenges = allRankingEntries.Sum(e => e.PointsFromChallenges);
            var totalPointsFromTournaments = allRankingEntries.Sum(e => e.PointsFromTournaments);
            var totalPointsFromClasses = allRankingEntries.Sum(e => e.PointsFromClasses);

            var averagePointsPerPlayer = activePlayers > 0 ? (double)totalPointsAwarded / activePlayers : 0;
            var challengeAcceptanceRate = totalChallenges > 0 ? (double)acceptedChallenges / totalChallenges * 100 : 0;
            var averagePointsPerChallenge = completedChallenges > 0 ? (double)totalPointsFromChallenges / completedChallenges : 0;

            var pointsBySource = new Dictionary<string, int>
            {
                { "Tournaments", totalPointsFromTournaments },
                { "Challenges", totalPointsFromChallenges },
                { "Classes", totalPointsFromClasses },
                { "MatchWins", allRankingEntries.Sum(e => e.PointsFromMatchWins) },
                { "MatchLosses", allRankingEntries.Sum(e => e.PointsFromMatchLosses) }
            };

            var statistics = new AnnualTableStatisticsDto
            {
                TotalPlayers = totalPlayers,
                ActivePlayers = activePlayers,
                TotalChallenges = totalChallenges,
                CompletedChallenges = completedChallenges,
                PendingChallenges = pendingChallenges,
                AcceptedChallenges = acceptedChallenges,
                RejectedChallenges = rejectedChallenges,
                TotalPointsAwarded = totalPointsAwarded,
                TotalPointsFromChallenges = totalPointsFromChallenges,
                TotalPointsFromTournaments = totalPointsFromTournaments,
                TotalPointsFromClasses = totalPointsFromClasses,
                AveragePointsPerPlayer = averagePointsPerPlayer,
                ChallengeAcceptanceRate = challengeAcceptanceRate,
                AveragePointsPerChallenge = averagePointsPerChallenge,
                PointsBySource = pointsBySource
            };

            // Actividad diaria del ranking
            var rankingTraces = await _context.RankingTraces
                .Include(t => t.AnnualTable)
                .Where(t => t.RecordedAt >= startDate && t.RecordedAt <= endDate)
                .ToListAsync();

            var dailyRankingActivity = rankingTraces
                .GroupBy(t => t.RecordedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyRankingActivityDto
                {
                    Date = g.Key,
                    PlayersWithActivity = g.Select(t => t.PlayerId).Distinct().Count(),
                    PointsAwarded = g.Sum(t => t.Points),
                    ChallengesCompleted = g.Count(t => t.Source == ScoringSource.Challenge && t.IsWin)
                })
                .ToList();

            // Distribución de puntos por fuente
            var totalPointsAllSources = pointsBySource.Values.Sum();
            var pointsDistributionBySource = pointsBySource
                .Select(kvp => new PointsDistributionBySourceDto
                {
                    Source = kvp.Key,
                    TotalPoints = kvp.Value,
                    Percentage = totalPointsAllSources > 0 ? (double)kvp.Value / totalPointsAllSources * 100 : 0
                })
                .ToList();

            // Top jugadores
            var topPlayers = allRankingEntries
                .OrderByDescending(e => e.PointsTotal)
                .Take(10)
                .Select((e, index) =>
                {
                    var player = playerDict.ContainsKey(e.PlayerId) ? playerDict[e.PlayerId] : null;
                    return new TopPlayerDto
                    {
                        PlayerId = e.PlayerId,
                        PlayerName = player?.Name,
                        PlayerSurname = player?.Surname,
                        Position = index + 1,
                        PointsTotal = e.PointsTotal,
                        Wins = e.Wins,
                        Losses = e.Losses,
                        PointsFromChallenges = e.PointsFromChallenges,
                        PointsFromTournaments = e.PointsFromTournaments
                    };
                })
                .ToList();

            // Estadísticas de desafíos por estado
            var challengeStatusDistribution = allChallenges
                .GroupBy(c => c.Status.ToString())
                .Select(g => new ChallengeStatusDistributionDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var challengeStatistics = challengeStatusDistribution
                .Select(c => new ChallengeStatisticsDto
                {
                    Status = c.Status,
                    Count = c.Count,
                    Percentage = totalChallenges > 0 ? (double)c.Count / totalChallenges * 100 : 0
                })
                .ToList();

            // Actividad diaria de desafíos
            var dailyChallengeActivity = allChallenges
                .GroupBy(c => c.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyChallengeActivityDto
                {
                    Date = g.Key,
                    ChallengesCreated = g.Count(),
                    ChallengesCompleted = g.Count(c => c.Status == Models.Challenge.ChallengeStatus.Played && c.PlayedAt.HasValue && c.PlayedAt.Value.Date == g.Key),
                    ChallengesAccepted = g.Count(c => c.Status == Models.Challenge.ChallengeStatus.Accepted && c.RespondedAt.HasValue && c.RespondedAt.Value.Date == g.Key)
                })
                .ToList();

            // Jugadores más desafiantes
            var mostChallengingPlayers = allChallenges
                .GroupBy(c => c.RequesterPlayerId)
                .Select(g =>
                {
                    var player = playerDict.ContainsKey(g.Key) ? playerDict[g.Key] : null;
                    var challengesWon = g.Count(c => c.Status == Models.Challenge.ChallengeStatus.Played && 
                                                      c.WinnerPlayerId == g.Key);
                    return new MostChallengingPlayerDto
                    {
                        PlayerId = g.Key,
                        PlayerName = player?.Name,
                        PlayerSurname = player?.Surname,
                        ChallengesCreated = g.Count(),
                        ChallengesWon = challengesWon,
                        WinRate = g.Count() > 0 ? (double)challengesWon / g.Count() * 100 : 0
                    };
                })
                .OrderByDescending(p => p.ChallengesCreated)
                .Take(10)
                .ToList();

            // Jugadores más desafiados
            var mostChallengedPlayers = allChallenges
                .GroupBy(c => c.TargetPlayerId)
                .Select(g =>
                {
                    var player = playerDict.ContainsKey(g.Key) ? playerDict[g.Key] : null;
                    var challengesAccepted = g.Count(c => c.Status == Models.Challenge.ChallengeStatus.Accepted || 
                                                          c.Status == Models.Challenge.ChallengeStatus.Played);
                    return new MostChallengedPlayerDto
                    {
                        PlayerId = g.Key,
                        PlayerName = player?.Name,
                        PlayerSurname = player?.Surname,
                        ChallengesReceived = g.Count(),
                        ChallengesAccepted = challengesAccepted,
                        AcceptanceRate = g.Count() > 0 ? (double)challengesAccepted / g.Count() * 100 : 0
                    };
                })
                .OrderByDescending(p => p.ChallengesReceived)
                .Take(10)
                .ToList();

            // Distribución de puntos por rango de posición
            var sortedEntries = allRankingEntries.OrderByDescending(e => e.PointsTotal).ToList();
            var positionRanges = new List<(string Range, Func<int, bool> Predicate)>
            {
                ("Top 1-5", pos => pos >= 1 && pos <= 5),
                ("Top 6-10", pos => pos >= 6 && pos <= 10),
                ("Top 11-20", pos => pos >= 11 && pos <= 20),
                ("Top 21-30", pos => pos >= 21 && pos <= 30),
                ("Resto", pos => pos > 30)
            };

            var pointsByPositionRange = positionRanges
                .Select(range =>
                {
                    var entriesInRange = sortedEntries
                        .Select((e, index) => new { Entry = e, Position = index + 1 })
                        .Where(x => range.Predicate(x.Position))
                        .ToList();

                    return new PointsByPositionRangeDto
                    {
                        Range = range.Range,
                        PlayerCount = entriesInRange.Count,
                        TotalPoints = entriesInRange.Sum(x => x.Entry.PointsTotal),
                        AveragePoints = entriesInRange.Count > 0 ? (double)entriesInRange.Sum(x => x.Entry.PointsTotal) / entriesInRange.Count : 0
                    };
                })
                .Where(r => r.PlayerCount > 0)
                .ToList();

            return new AnnualTableReportDto
            {
                Statistics = statistics,
                DailyRankingActivity = dailyRankingActivity,
                PointsDistributionBySource = pointsDistributionBySource,
                TopPlayers = topPlayers,
                ChallengeStatistics = challengeStatistics,
                DailyChallengeActivity = dailyChallengeActivity,
                ChallengeStatusDistribution = challengeStatusDistribution,
                MostChallengingPlayers = mostChallengingPlayers,
                MostChallengedPlayers = mostChallengedPlayers,
                PointsByPositionRange = pointsByPositionRange
            };
        }
    }
}

