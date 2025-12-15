namespace padelya_api.DTOs.Annual
{
    public class AnnualTableReportDto
    {
        public AnnualTableStatisticsDto Statistics { get; set; } = new();
        public List<DailyRankingActivityDto> DailyRankingActivity { get; set; } = new();
        public List<PointsDistributionBySourceDto> PointsDistributionBySource { get; set; } = new();
        public List<TopPlayerDto> TopPlayers { get; set; } = new();
        public List<ChallengeStatisticsDto> ChallengeStatistics { get; set; } = new();
        public List<DailyChallengeActivityDto> DailyChallengeActivity { get; set; } = new();
        public List<ChallengeStatusDistributionDto> ChallengeStatusDistribution { get; set; } = new();
        public List<MostChallengingPlayerDto> MostChallengingPlayers { get; set; } = new();
        public List<MostChallengedPlayerDto> MostChallengedPlayers { get; set; } = new();
        public List<PointsByPositionRangeDto> PointsByPositionRange { get; set; } = new();
    }

    public class DailyRankingActivityDto
    {
        public DateTime Date { get; set; }
        public int PlayersWithActivity { get; set; }
        public int PointsAwarded { get; set; }
        public int ChallengesCompleted { get; set; }
    }

    public class PointsDistributionBySourceDto
    {
        public string Source { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public double Percentage { get; set; }
    }

    public class TopPlayerDto
    {
        public int PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerSurname { get; set; }
        public int Position { get; set; }
        public int PointsTotal { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointsFromChallenges { get; set; }
        public int PointsFromTournaments { get; set; }
    }

    public class ChallengeStatisticsDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class DailyChallengeActivityDto
    {
        public DateTime Date { get; set; }
        public int ChallengesCreated { get; set; }
        public int ChallengesCompleted { get; set; }
        public int ChallengesAccepted { get; set; }
    }

    public class ChallengeStatusDistributionDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MostChallengingPlayerDto
    {
        public int PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerSurname { get; set; }
        public int ChallengesCreated { get; set; }
        public int ChallengesWon { get; set; }
        public double WinRate { get; set; }
    }

    public class MostChallengedPlayerDto
    {
        public int PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerSurname { get; set; }
        public int ChallengesReceived { get; set; }
        public int ChallengesAccepted { get; set; }
        public double AcceptanceRate { get; set; }
    }

    public class PointsByPositionRangeDto
    {
        public string Range { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public int TotalPoints { get; set; }
        public double AveragePoints { get; set; }
    }
}

