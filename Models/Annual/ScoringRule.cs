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

        /// <summary>
        /// Configuración adicional en formato JSON para estrategias específicas
        /// Ej: Para Challenge: { "top5Bonus": 1.5, "top10Bonus": 1.3, "top20Bonus": 1.15 }
        /// Ej: Para Tournament: { "1ra": 2.0, "2da": 1.5, "3ra": 1.2, "4ta": 1.0, "5ta": 0.8 }
        /// </summary>
        public string? ConfigurationJson { get; set; }
    }
}

