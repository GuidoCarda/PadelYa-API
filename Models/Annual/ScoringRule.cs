using System;

namespace padelya_api.Models.Annual
{
    public class ScoringRule
    {
        public int Id { get; set; }
        public int AnnualTableId { get; set; }
        public AnnualTable AnnualTable { get; set; }

        public ScoringSource Source { get; set; }

        // Para simplicidad inicial: base points y multiplicadores/caps simples
        public int BasePoints { get; set; }
        public float Multiplier { get; set; } = 1.0f;
        public int? MaxPoints { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeactivatedAt { get; set; }
    }
}

