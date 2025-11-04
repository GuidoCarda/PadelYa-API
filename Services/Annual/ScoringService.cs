using System.Collections.Generic;
using System.Linq;
using padelya_api.Models.Annual;
using padelya_api.Services.Annual.Scoring;

namespace padelya_api.Services.Annual
{
    public class ScoringService : IScoringService
    {
        private readonly Dictionary<ScoringSource, IScoringStrategy> _strategies;

        public ScoringService(IEnumerable<IScoringStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.Source, s => s);
        }

        public int ComputePoints(ScoringSource source, ScoringRule rule, object context)
        {
            if (_strategies.TryGetValue(source, out var strategy))
            {
                return strategy.CalculatePoints(rule, context);
            }
            return 0;
        }
    }
}

