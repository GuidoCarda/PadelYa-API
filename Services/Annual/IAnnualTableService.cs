using System.Collections.Generic;
using System.Threading.Tasks;
using padelya_api.DTOs.Annual;
using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual
{
    public interface IAnnualTableService
    {
        Task<AnnualTable> GetOrCreateForYearAsync(int year);
        Task<AnnualTableStatus> GetStatusAsync(int year);
        Task<List<RankingEntry>> GetRankingAsync(int year, int? playerId = null, string? from = null, string? to = null);
        Task<List<RankingEntryDto>> GetRankingWithNamesAsync(int year, int? playerId = null, string? from = null, string? to = null);
        Task<AnnualTableStatisticsDto> GetStatisticsAsync(int year);
        Task<AnnualTable> UpdateStatusAsync(int year, AnnualTableStatus status);
        Task<List<ScoringRuleDto>> GetScoringRulesAsync(int year);
        Task<List<ScoringRuleDto>> UpsertScoringRulesAsync(int year, List<ScoringRuleDto> rules);
        Task ApplyPointsAsync(int year, int playerId, ScoringSource source, int points, bool isWin, int? matchId = null, string? matchType = null, string? scoringStrategy = null, int? recordedByUserId = null, string? metadata = null);
        Task<AnnualTableReportDto> GetAnnualTableReportAsync(DateTime startDate, DateTime endDate);
    }
}
