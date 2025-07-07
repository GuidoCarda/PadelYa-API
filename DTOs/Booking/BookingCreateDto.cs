namespace padelya_api.DTOs.Booking
{
  public class BookingCreateDto
  {
    public int CourtId { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int PersonId { get; set; }
    public string PaymentType { get; set; } // 'deposit' o 'total'
  }
}