using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual.Scoring
{
    public interface IScoringStrategy
    {
        ScoringSource Source { get; }
        int CalculatePoints(ScoringRule rule, object context);
    }
}

