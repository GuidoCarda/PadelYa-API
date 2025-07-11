using System.Collections.Generic;
using padelya_api.Constants;
using padelya_api.DTOs.Payment;

namespace padelya_api.DTOs.Booking
{
  public class BookingDto
  {
    public int Id { get; set; }
    public int CourtSlotId { get; set; }
    public int PersonId { get; set; }


    public BookingStatus Status { get; set; }
    public string DisplayStatus { get; set; } = string.Empty;

    // Información del slot/cancha
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int CourtId { get; set; }
    public string CourtName { get; set; }
    public string CourtType { get; set; }

    // Información del usuario
    public string UserName { get; set; }
    public string UserSurname { get; set; }
    public string UserEmail { get; set; }

    // Información de pagos
    public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    public decimal TotalPaid { get; set; }
    public decimal TotalAmount { get; set; }
  }
}

