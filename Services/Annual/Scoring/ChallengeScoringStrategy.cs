using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual.Scoring
{
    public class ChallengeScoringStrategy : IScoringStrategy
    {
        public ScoringSource Source => ScoringSource.Challenge;

        public int CalculatePoints(ScoringRule rule, object context)
        {
            // Por ahora: puntos base * multiplicador, con tope
            var points = (int)(rule.BasePoints * rule.Multiplier);
            if (rule.MaxPoints.HasValue && points > rule.MaxPoints.Value)
            {
                points = rule.MaxPoints.Value;
            }
            return points;
        }
    }
}

