namespace padelya_api.DTOs.Booking
{
  public class BookingReportDto
  {
    public BookingStatisticsDto Statistics { get; set; }
    public List<DailyRevenueDto> DailyRevenue { get; set; }
    public List<TimeSlotDistributionDto> TimeSlotDistribution { get; set; }
    public List<CourtPopularityDto> CourtPopularity { get; set; }
    public List<BookingStatusDistributionDto> StatusDistribution { get; set; }
    public List<PaymentMethodDistributionDto> PaymentMethodDistribution { get; set; }
  }

  public class BookingStatisticsDto
  {
    public decimal TotalRevenue { get; set; }
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int PendingBookings { get; set; }
    public decimal AverageRevenuePerBooking { get; set; }
    public decimal OccupancyRate { get; set; }
    public decimal CancellationRate { get; set; }
  }

  public class DailyRevenueDto
  {
    public string Date { get; set; }
    public decimal Revenue { get; set; }
    public int BookingCount { get; set; }
  }

  public class TimeSlotDistributionDto
  {
    public string TimeSlot { get; set; }
    public int BookingCount { get; set; }
  }

  public class CourtPopularityDto
  {
    public int CourtId { get; set; }
    public string CourtName { get; set; }
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
  }

  public class BookingStatusDistributionDto
  {
    public string Status { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
  }

  public class PaymentMethodDistributionDto
  {
    public string PaymentMethod { get; set; }
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
  }
}

