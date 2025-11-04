using padelya_api.Models.Annual;

namespace padelya_api.Services.Annual
{
    public interface IScoringService
    {
        int ComputePoints(ScoringSource source, ScoringRule rule, object context);
    }
}

