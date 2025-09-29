using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
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

                // Seed default scoring rules
                var defaultRules = new List<ScoringRule>
                {
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.Challenge, BasePoints = 20, Multiplier = 1.0f, MaxPoints = null, IsActive = true },
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.Tournament, BasePoints = 50, Multiplier = 1.0f, MaxPoints = null, IsActive = true },
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.Class, BasePoints = 5, Multiplier = 1.0f, MaxPoints = 10, IsActive = true },
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.MatchWin, BasePoints = 10, Multiplier = 1.0f, MaxPoints = null, IsActive = true },
                    new ScoringRule { AnnualTableId = table.Id, Source = ScoringSource.MatchLoss, BasePoints = 2, Multiplier = 1.0f, MaxPoints = null, IsActive = true },
                };
                await _context.ScoringRules.AddRangeAsync(defaultRules);

                // Seed sample ranking entries for existing players (if available)
                var playerIds = await _context.Players.Select(p => p.Id).ToListAsync();
                var demoPoints = new int[] { 120, 90, 60, 40, 20 };
                int i = 0;
                foreach (var pid in playerIds)
                {
                    var entry = new RankingEntry
                    {
                        AnnualTableId = table.Id,
                        PlayerId = pid,
                        PointsTotal = i < demoPoints.Length ? demoPoints[i] : 0,
                        PointsFromChallenges = i < demoPoints.Length ? demoPoints[i] : 0
                    };
                    i++;
                    _context.RankingEntries.Add(entry);
                }

                await _context.SaveChangesAsync();
            }
            return table;
        }

        public async Task<List<RankingEntry>> GetRankingAsync(int year, int? playerId = null, string? from = null, string? to = null)
        {
            await GetOrCreateForYearAsync(year);

            var query = _context.RankingEntries
                .Where(e => e.AnnualTable.Year == year)
                .Include(e => e.AnnualTable)
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

        public async Task<List<ScoringRule>> GetScoringRulesAsync(int year)
        {
            var table = await GetOrCreateForYearAsync(year);
            return await _context.ScoringRules
                .Where(r => r.AnnualTableId == table.Id)
                .ToListAsync();
        }

        public async Task<List<ScoringRule>> UpsertScoringRulesAsync(int year, List<ScoringRule> rules)
        {
            var table = await GetOrCreateForYearAsync(year);

            var existing = await _context.ScoringRules
                .Where(r => r.AnnualTableId == table.Id)
                .ToListAsync();

            _context.ScoringRules.RemoveRange(existing);
            foreach (var r in rules)
            {
                r.Id = 0;
                r.AnnualTableId = table.Id;
            }
            await _context.ScoringRules.AddRangeAsync(rules);
            await _context.SaveChangesAsync();
            return rules;
        }

        public async Task ApplyPointsAsync(int year, int playerId, ScoringSource source, int points, bool isWin)
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

            await _context.SaveChangesAsync();
        }
    }
}

