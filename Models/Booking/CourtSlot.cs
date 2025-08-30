using padelya_api.models;
using padelya_api.Models;
using padelya_api.Models.Tournament;

public enum CourtSlotStatus
{
  Active,           // Slot activo (reserva/clase/torneo en curso)
  Pending,          // Slot pendiente (pendiente de pago para confirmar la reserva)
  Cancelled,        // Slot cancelado pero mantiene información
}

public class CourtSlot
{
  public int Id { get; set; }
  public int CourtId { get; set; }
  public Court Court { get; set; }
  public DateTime Date { get; set; }
  public TimeOnly StartTime { get; set; }
  public TimeOnly EndTime { get; set; }

  public CourtSlotStatus Status { get; set; } = CourtSlotStatus.Active;

  public DateTime? ExpiresAt { get; set; }

  // Relaciones 1:1 (solo una será no nula)
  public Booking Booking { get; set; }
  public Lesson Lesson { get; set; }
  public TournamentMatch TournamentMatch { get; set; }


  public bool IsCompleted => Date.Add(StartTime.ToTimeSpan()) <= DateTime.Now;
  public bool IsActive => !IsCompleted && Status == CourtSlotStatus.Active;
  public bool IsCancelled => Status == CourtSlotStatus.Cancelled;
  public bool IsPending => Status == CourtSlotStatus.Pending;
  public bool IsExpired => IsPending && ExpiresAt != null && ExpiresAt <= DateTime.Now;
}