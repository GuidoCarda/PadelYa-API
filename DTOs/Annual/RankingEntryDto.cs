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
        public int PendingChallenges { get; set; }
        public int TotalPointsAwarded { get; set; }
        public Dictionary<string, int> PointsBySource { get; set; } = new();
    }
}

