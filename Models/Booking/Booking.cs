using padelya_api.Constants;
using padelya_api.Models;

namespace padelya_api.Models
{
  public class Booking
  {

    public int Id { get; set; }
    public int CourtSlotId { get; set; }
    public CourtSlot CourtSlot { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; }


    // Pagos (seña, saldo, total, etc.)
    public List<Payment> Payments { get; set; } = [];

    public BookingStatus Status { get; set; } = BookingStatus.ReservedDeposit;


    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }


    public bool IsCompleted => CourtSlot?.Date.Add(CourtSlot.StartTime.ToTimeSpan()) <= DateTime.Now;
    public bool IsActive => !IsCompleted && (Status != BookingStatus.CancelledByAdmin || Status != BookingStatus.CancelledByClient);
    public decimal PendingAmount => CourtSlot.Court.BookingPrice - Payments.Sum(p => p.Amount);


    public string DisplayStatus => Status switch
    {
      BookingStatus.ReservedPaid => "Pagada",
      BookingStatus.ReservedDeposit => "Señada",
      BookingStatus.CancelledByClient => "Cancelada por cliente",
      BookingStatus.CancelledByAdmin => "Cancelada por admin",
      _ => "Desconocida"
    };

  }
}