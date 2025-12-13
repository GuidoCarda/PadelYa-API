using System.Text.Json;
using padelya_api.DTOs.Annual;
using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual.Scoring
{
    public class TournamentScoringStrategy : IScoringStrategy
    {
        public ScoringSource Source => ScoringSource.Tournament;

        public int CalculatePoints(ScoringRule rule, object context)
        {
            // Calcular puntos base
            var points = (int)(rule.BasePoints * rule.Multiplier);

            // Si el contexto contiene información sobre la categoría del torneo, aplicar multiplicador
            if (context is TournamentScoringContext tournamentContext)
            {
                // Cargar configuración desde JSON o usar valores por defecto
                var config = GetConfiguration(rule);
                
                var categoryMultiplier = tournamentContext.Category.ToLower() switch
                {
                    "1ra" or "primera" or "1" => config.Category1ra,
                    "2da" or "segunda" or "2" => config.Category2da,
                    "3ra" or "tercera" or "3" => config.Category3ra,
                    "4ta" or "cuarta" or "4" => config.Category4ta,
                    "5ta" or "quinta" or "5" => config.Category5ta,
                    _ => 1.0f // Por defecto sin modificación
                };

                points = (int)(points * categoryMultiplier);
            }

            // Aplicar tope máximo si existe
            if (rule.MaxPoints.HasValue && points > rule.MaxPoints.Value)
            {
                points = rule.MaxPoints.Value;
            }

            return points;
        }

        private TournamentScoringConfiguration GetConfiguration(ScoringRule rule)
        {
            if (string.IsNullOrWhiteSpace(rule.ConfigurationJson))
            {
                return new TournamentScoringConfiguration(); // Valores por defecto
            }

            try
            {
                return JsonSerializer.Deserialize<TournamentScoringConfiguration>(rule.ConfigurationJson) 
                    ?? new TournamentScoringConfiguration();
            }
            catch
            {
                return new TournamentScoringConfiguration(); // Si hay error, usar valores por defecto
            }
        }
    }

    public class TournamentScoringContext
    {
        public string Category { get; set; } = string.Empty;
        public string? TournamentTitle { get; set; }
    }
}

