using padelya_api.Models.Annual;
using System;

namespace padelya_api.DTOs.Annual
{
    public class RankingTraceDto
    {
        public int Id { get; set; }
        public int? MatchId { get; set; }
        public string MatchType { get; set; } = string.Empty;
        public int Points { get; set; }
        public int AnnualTableId { get; set; }
        public int? Year { get; set; }
        public int RankingEntryId { get; set; }
        public int PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerSurname { get; set; }
        public ScoringSource Source { get; set; }
        public string ScoringStrategy { get; set; } = string.Empty;
        public bool IsWin { get; set; }
        public DateTime RecordedAt { get; set; }
        public int? RecordedByUserId { get; set; }
        public string? RecordedByUserName { get; set; }
        public string? Metadata { get; set; }
    }
}

