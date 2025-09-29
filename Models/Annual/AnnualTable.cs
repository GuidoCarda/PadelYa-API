using System;
using System.Collections.Generic;

namespace padelya_api.Models.Annual
{
    public class AnnualTable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public AnnualTableStatus Status { get; set; } = AnnualTableStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SuspendedAt { get; set; }
        public DateTime? ResumedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public List<RankingEntry> Entries { get; set; } = new();
        public List<ScoringRule> ScoringRules { get; set; } = new();
    }
}

