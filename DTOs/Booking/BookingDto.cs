using System.Collections.Generic;
using padelya_api.DTOs.Payment;

namespace padelya_api.DTOs.Booking
{
  public class BookingDto
  {
    public int Id { get; set; }
    public int CourtSlotId { get; set; }
    public int PersonId { get; set; }
    public List<PaymentDto> Payments { get; set; }
    // Otros campos relevantes
  }
}

