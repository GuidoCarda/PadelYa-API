using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Annual;
using padelya_api.Models;
using padelya_api.Models.Annual;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace padelya_api.Services.Annual
{
    public class RankingTraceService : IRankingTraceService
    {
        private readonly PadelYaDbContext _context;

        public RankingTraceService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<List<RankingTraceDto>> GetTracesByPlayerAsync(int playerId, int? year = null)
        {
            var query = _context.RankingTraces
                .Include(t => t.AnnualTable)
                .Include(t => t.RankingEntry)
                .Where(t => t.PlayerId == playerId);

            if (year.HasValue)
            {
                query = query.Where(t => t.AnnualTable.Year == year.Value);
            }

            var traces = await query
                .OrderByDescending(t => t.RecordedAt)
                .ToListAsync();

            return await MapToDtoAsync(traces);
        }

        public async Task<List<RankingTraceDto>> GetTracesByMatchAsync(int matchId, string matchType)
        {
            var traces = await _context.RankingTraces
                .Include(t => t.AnnualTable)
                .Include(t => t.RankingEntry)
                .Where(t => t.MatchId == matchId && t.MatchType == matchType)
                .OrderByDescending(t => t.RecordedAt)
                .ToListAsync();

            return await MapToDtoAsync(traces);
        }

        public async Task<List<RankingTraceDto>> GetTracesByRankingEntryAsync(int rankingEntryId)
        {
            var traces = await _context.RankingTraces
                .Include(t => t.AnnualTable)
                .Include(t => t.RankingEntry)
                .Where(t => t.RankingEntryId == rankingEntryId)
                .OrderByDescending(t => t.RecordedAt)
                .ToListAsync();

            return await MapToDtoAsync(traces);
        }

        public async Task<List<RankingTraceDto>> GetTracesByYearAsync(int year)
        {
            var traces = await _context.RankingTraces
                .Include(t => t.AnnualTable)
                .Include(t => t.RankingEntry)
                .Where(t => t.AnnualTable.Year == year)
                .OrderByDescending(t => t.RecordedAt)
                .ToListAsync();

            return await MapToDtoAsync(traces);
        }

        public async Task<List<RankingTraceDto>> GetTracesByChallengeAsync(int challengeId)
        {
            var traces = await _context.RankingTraces
                .Include(t => t.AnnualTable)
                .Include(t => t.RankingEntry)
                .Where(t => t.MatchId == challengeId && t.MatchType == "Challenge")
                .OrderByDescending(t => t.RecordedAt)
                .ToListAsync();

            return await MapToDtoAsync(traces);
        }

        private async Task<List<RankingTraceDto>> MapToDtoAsync(List<RankingTrace> traces)
        {
            var playerIds = traces.Select(t => t.PlayerId).Distinct().ToList();
            var userIds = traces.Where(t => t.RecordedByUserId.HasValue)
                .Select(t => t.RecordedByUserId!.Value)
                .Distinct()
                .ToList();

            var users = await _context.Users
                .Where(u => (u.PersonId.HasValue && playerIds.Contains(u.PersonId.Value)) || userIds.Contains(u.Id))
                .ToListAsync();

            return traces.Select(t =>
            {
                var playerUser = users.FirstOrDefault(u => u.PersonId == t.PlayerId);
                var recordedByUser = t.RecordedByUserId.HasValue
                    ? users.FirstOrDefault(u => u.Id == t.RecordedByUserId.Value)
                    : null;

                return new RankingTraceDto
                {
                    Id = t.Id,
                    MatchId = t.MatchId,
                    MatchType = t.MatchType,
                    Points = t.Points,
                    AnnualTableId = t.AnnualTableId,
                    Year = t.AnnualTable?.Year,
                    RankingEntryId = t.RankingEntryId,
                    PlayerId = t.PlayerId,
                    PlayerName = playerUser?.Name,
                    PlayerSurname = playerUser?.Surname,
                    Source = t.Source,
                    ScoringStrategy = t.ScoringStrategy,
                    IsWin = t.IsWin,
                    RecordedAt = t.RecordedAt,
                    RecordedByUserId = t.RecordedByUserId,
                    RecordedByUserName = recordedByUser != null ? $"{recordedByUser.Name} {recordedByUser.Surname}" : null,
                    Metadata = t.Metadata
                };
            }).ToList();
        }
    }
}

