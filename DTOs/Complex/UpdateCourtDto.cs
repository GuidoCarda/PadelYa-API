using padelya_api.Constants;

namespace padelya_api.DTOs
{
  public class UpdateCourtDto
  {
    public string? Name { get; set; }
    public CourtStatus? CourtStatus { get; set; }
    public int? BookingPrice { get; set; }
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
    public string? Type { get; set; }
  }
}