using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual.Scoring
{
    public class MatchWinScoringStrategy : IScoringStrategy
    {
        public ScoringSource Source => ScoringSource.MatchWin;

        public int CalculatePoints(ScoringRule rule, object context)
        {
            var points = (int)(rule.BasePoints * rule.Multiplier);
            if (rule.MaxPoints.HasValue && points > rule.MaxPoints.Value)
            {
                points = rule.MaxPoints.Value;
            }
            return points;
        }
    }
}

