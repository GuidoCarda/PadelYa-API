using padelya_api.DTOs.Payment;

namespace padelya_api.DTOs.Booking
{
  public class BookingResponseDto
  {
    public BookingDto Booking { get; set; }
    public PaymentDto Payment { get; set; }
  }
}