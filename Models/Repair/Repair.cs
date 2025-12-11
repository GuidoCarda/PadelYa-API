using System.ComponentModel.DataAnnotations.Schema;
using padelya_api.Models.Repair.States;

namespace padelya_api.Models.Repair
{
  public class Repair
  {
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? EstimatedCompletionTime { get; set; }

    public decimal Price { get; set; }
    public string DamageDescription { get; set; } = String.Empty;
    public string RepairNotes { get; set; } = String.Empty;

    // For db persistence
    public RepairStatus Status { get; set; } = RepairStatus.Received;

    //For runtine behaviour, this is not mapped to DB
    [NotMapped]
    public IRepairState State { get; set; }

    // Navigation properties
    public int RacketId { get; set; }
    public Racket Racket { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; }

    public int? PaymentId { get; set; }
    public Payment? Payment { get; set; }

    // Audit trail navigation property
    public ICollection<RepairAudit> Audits { get; set; } = new List<RepairAudit>();

    public void AdvanceRepairProcess() => State.AdvanceState(this);
    public void CancelRepair() => State.CancelRepair(this);
    public string GetCurrentStatus() => State.GetStatusName();

    // Helper to initialize state from status enum
    public void InitializeState()
    {
      State = Status switch
      {
        RepairStatus.Received => new ReceivedState(),
        RepairStatus.InRepair => new InRepairState(),
        RepairStatus.ReadyForPickup => new ReadyForPickupState(),
        RepairStatus.Delivered => new DeliveredState(),
        RepairStatus.Cancelled => new CancelledState(),
        _ => throw new InvalidOperationException($"Unknown status: {Status}")
      };
    }
  }
}
