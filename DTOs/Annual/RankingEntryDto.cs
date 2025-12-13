namespace padelya_api.DTOs.Annual
{
    public class RankingEntryDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerSurname { get; set; }
        public int Position { get; set; }
        public int PointsTotal { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int PointsFromTournaments { get; set; }
        public int PointsFromChallenges { get; set; }
        public int PointsFromClasses { get; set; }
        public int PointsFromMatchWins { get; set; }
        public int PointsFromMatchLosses { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }

    public class AnnualTableStatisticsDto
    {
        public int TotalPlayers { get; set; }
        public int ActivePlayers { get; set; }
        public int TotalChallenges { get; set; }
        public int CompletedChallenges { get; set; }
        public int PendingChallenges { get; set; }
        public int AcceptedChallenges { get; set; }
        public int RejectedChallenges { get; set; }
        public int TotalPointsAwarded { get; set; }
        public int TotalPointsFromChallenges { get; set; }
        public int TotalPointsFromTournaments { get; set; }
        public int TotalPointsFromClasses { get; set; }
        public double AveragePointsPerPlayer { get; set; }
        public double ChallengeAcceptanceRate { get; set; }
        public double AveragePointsPerChallenge { get; set; }
        public Dictionary<string, int> PointsBySource { get; set; } = new();
    }
}

