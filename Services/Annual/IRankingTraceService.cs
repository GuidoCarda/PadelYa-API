using System.Collections.Generic;
using System.Threading.Tasks;
using padelya_api.DTOs.Annual;

namespace padelya_api.Services.Annual
{
    public interface IRankingTraceService
    {
        Task<List<RankingTraceDto>> GetTracesByPlayerAsync(int playerId, int? year = null);
        Task<List<RankingTraceDto>> GetTracesByMatchAsync(int matchId, string matchType);
        Task<List<RankingTraceDto>> GetTracesByRankingEntryAsync(int rankingEntryId);
        Task<List<RankingTraceDto>> GetTracesByYearAsync(int year);
        Task<List<RankingTraceDto>> GetTracesByChallengeAsync(int challengeId);
    }
}

