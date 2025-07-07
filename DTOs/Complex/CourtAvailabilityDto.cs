using System.Collections.Generic;

namespace padelya_api.DTOs.Complex
{
  public class CourtAvailabilityDto
  {
    public int CourtId { get; set; }
    public string CourtName { get; set; }
    public string Type { get; set; }
    public int BookingPrice { get; set; }
    public List<SlotAvailabilityDto> Slots { get; set; }
  }

  public class SlotAvailabilityDto
  {
    public string Start { get; set; } // "HH:mm"
    public string End { get; set; }   // "HH:mm"
    public string Status { get; set; } // "available" | "booked"
  }
}