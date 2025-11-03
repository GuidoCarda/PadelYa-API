using System;
using System.Collections.Generic;

namespace padelya_api.Models.Annual
{
    /// <summary>
    /// Entidad de trazabilidad para auditar cambios en el ranking anual
    /// Registra cada acción que afecta a RankingEntry
    /// </summary>
    public class RankingTrace
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Identificador del partido (ChallengeId o TournamentMatchId)
        /// </summary>
        public int? MatchId { get; set; }
        
        /// <summary>
        /// Tipo de partido (Challenge, Tournament, etc.)
        /// </summary>
        public string MatchType { get; set; } = string.Empty;
        
        /// <summary>
        /// Puntos aplicados en esta acción
        /// </summary>
        public int Points { get; set; }
        
        /// <summary>
        /// ID de la tabla anual afectada
        /// </summary>
        public int AnnualTableId { get; set; }
        public AnnualTable AnnualTable { get; set; }
        
        /// <summary>
        /// ID de la entrada en el ranking afectada
        /// </summary>
        public int RankingEntryId { get; set; }
        public RankingEntry RankingEntry { get; set; }
        
        /// <summary>
        /// ID del jugador afectado
        /// </summary>
        public int PlayerId { get; set; }
        
        /// <summary>
        /// Fuente de los puntos (Challenge, Tournament, Class, etc.)
        /// </summary>
        public ScoringSource Source { get; set; }
        
        /// <summary>
        /// Estrategia utilizada para calcular puntos
        /// </summary>
        public string ScoringStrategy { get; set; } = string.Empty;
        
        /// <summary>
        /// Indica si fue una victoria o derrota
        /// </summary>
        public bool IsWin { get; set; }
        
        /// <summary>
        /// Fecha y hora exacta en que se registró
        /// </summary>
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// ID del usuario que registró la acción (si aplica)
        /// </summary>
        public int? RecordedByUserId { get; set; }
        
        /// <summary>
        /// Información adicional en formato JSON para flexibilidad
        /// </summary>
        public string? Metadata { get; set; }
    }
}

