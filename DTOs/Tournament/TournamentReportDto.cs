namespace padelya_api.DTOs.Tournament
{
  public class TournamentReportDto
  {
    public TournamentStatisticsDto Statistics { get; set; } = new();
    public List<DailyTournamentRevenueDto> DailyRevenue { get; set; } = new();
    public List<TournamentStatusDistributionDto> StatusDistribution { get; set; } = new();
    public List<TournamentCategoryAnalysisDto> CategoryAnalysis { get; set; } = new();
    public List<TournamentPopularityDto> TournamentPopularity { get; set; } = new();
    public List<EnrollmentStatusDistributionDto> EnrollmentStatusDistribution { get; set; } = new();
  }

  public class TournamentStatisticsDto
  {
    public decimal TotalRevenue { get; set; }
    public int TotalTournaments { get; set; }
    public int FinishedTournaments { get; set; }
    public int TotalEnrollments { get; set; }
    public int ConfirmedEnrollments { get; set; }
    public int CancelledEnrollments { get; set; }
    public int RejectedEnrollments { get; set; }
    public decimal AverageRevenuePerTournament { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal OccupancyRate { get; set; }
    public decimal CancellationRate { get; set; }
    public decimal AverageEnrollmentsPerTournament { get; set; }
    public int ExpiredEnrollments { get; set; }
  }

  public class DailyTournamentRevenueDto
  {
    public string Date { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int EnrollmentCount { get; set; }
  }

  public class TournamentStatusDistributionDto
  {
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
  }

  public class TournamentCategoryAnalysisDto
  {
    public string Category { get; set; } = string.Empty;
    public int TournamentCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalEnrollments { get; set; }
    public decimal AverageEnrollmentsPerTournament { get; set; }
  }

  public class TournamentPopularityDto
  {
    public int TournamentId { get; set; }
    public string TournamentTitle { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal OccupancyRate { get; set; }
    public DateTime TournamentStartDate { get; set; }
  }

  public class EnrollmentStatusDistributionDto
  {
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
  }
}


