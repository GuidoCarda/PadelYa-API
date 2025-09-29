using System.Collections.Generic;
using System.Threading.Tasks;
using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual
{
    public interface IAnnualTableService
    {
        Task<AnnualTable> GetOrCreateForYearAsync(int year);
        Task<List<RankingEntry>> GetRankingAsync(int year, int? playerId = null, string? from = null, string? to = null);
        Task<AnnualTable> UpdateStatusAsync(int year, AnnualTableStatus status);
        Task<List<ScoringRule>> GetScoringRulesAsync(int year);
        Task<List<ScoringRule>> UpsertScoringRulesAsync(int year, List<ScoringRule> rules);
        Task ApplyPointsAsync(int year, int playerId, ScoringSource source, int points, bool isWin);
    }
}

