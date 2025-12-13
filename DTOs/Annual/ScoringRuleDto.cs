using padelya_api.Models.Annual;

namespace padelya_api.DTOs.Annual
{
    public class ScoringRuleDto
    {
        public int Id { get; set; }
        public int AnnualTableId { get; set; }
        public ScoringSource Source { get; set; }
        public int BasePoints { get; set; }
        public float Multiplier { get; set; }
        public int? MaxPoints { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public string? ConfigurationJson { get; set; }
    }

    public class ChallengeScoringConfiguration
    {
        public float Top5Bonus { get; set; } = 1.5f;      // +50%
        public float Top10Bonus { get; set; } = 1.3f;     // +30%
        public float Top20Bonus { get; set; } = 1.15f;   // +15%
    }

    public class TournamentScoringConfiguration
    {
        public float Category1ra { get; set; } = 2.0f;
        public float Category2da { get; set; } = 1.5f;
        public float Category3ra { get; set; } = 1.2f;
        public float Category4ta { get; set; } = 1.0f;
        public float Category5ta { get; set; } = 0.8f;
    }
}

