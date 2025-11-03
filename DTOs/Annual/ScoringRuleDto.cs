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
    }
}

