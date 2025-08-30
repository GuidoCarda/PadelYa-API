using padelya_api.Constants;

namespace padelya_api.DTOs.Booking
{
  public class BookingReserveWithPaymentDto
  {
    public int CourtId { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int PersonId { get; set; }
    public PaymentType PaymentType { get; set; } // deposit, total, balance
  }
}