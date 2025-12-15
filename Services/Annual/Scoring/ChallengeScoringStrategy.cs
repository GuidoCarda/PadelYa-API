using System.Text.Json;
using padelya_api.DTOs.Annual;
using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual.Scoring
{
    public class ChallengeScoringStrategy : IScoringStrategy
    {
        public ScoringSource Source => ScoringSource.Challenge;

        public int CalculatePoints(ScoringRule rule, object context)
        {
            // Calcular puntos base
            var points = (int)(rule.BasePoints * rule.Multiplier);

            // Si el contexto contiene información sobre la posición del rival, aplicar bonificación
            if (context is ChallengeScoringContext challengeContext)
            {
                // Cargar configuración desde JSON o usar valores por defecto
                var config = GetConfiguration(rule);
                
                var rivalPosition = challengeContext.RivalPosition;
                if (rivalPosition <= 5)
                {
                    points = (int)(points * config.Top5Bonus);
                }
                else if (rivalPosition <= 10)
                {
                    points = (int)(points * config.Top10Bonus);
                }
                else if (rivalPosition <= 20)
                {
                    points = (int)(points * config.Top20Bonus);
                }
            }

            // Aplicar tope máximo si existe
            if (rule.MaxPoints.HasValue && points > rule.MaxPoints.Value)
            {
                points = rule.MaxPoints.Value;
            }

            return points;
        }

        private ChallengeScoringConfiguration GetConfiguration(ScoringRule rule)
        {
            if (string.IsNullOrWhiteSpace(rule.ConfigurationJson))
            {
                return new ChallengeScoringConfiguration(); // Valores por defecto
            }

            try
            {
                return JsonSerializer.Deserialize<ChallengeScoringConfiguration>(rule.ConfigurationJson) 
                    ?? new ChallengeScoringConfiguration();
            }
            catch
            {
                return new ChallengeScoringConfiguration(); // Si hay error, usar valores por defecto
            }
        }
    }

    public class ChallengeScoringContext
    {
        public int RivalPosition { get; set; }
        public int WinnerPosition { get; set; }
    }
}

