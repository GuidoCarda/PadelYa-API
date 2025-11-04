using System;

namespace padelya_api.Models.Annual
{
    public class RankingEntry
    {
        public int Id { get; set; }
        public int AnnualTableId { get; set; }
        public AnnualTable AnnualTable { get; set; }

        public int PlayerId { get; set; }

        public int PointsTotal { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        public int PointsFromTournaments { get; set; }
        public int PointsFromChallenges { get; set; }
        public int PointsFromClasses { get; set; }
        public int PointsFromMatchWins { get; set; }
        public int PointsFromMatchLosses { get; set; }

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

