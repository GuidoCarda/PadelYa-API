using padelya_api.Models.Repair.States;

namespace padelya_api.Models.Repair
{
  public class RepairAudit
  {
    public int Id { get; set; }

    public int RepairId { get; set; }
    public Repair Repair { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public RepairAuditAction Action { get; set; }
    public DateTime Timestamp { get; set; }

    // Store the status change if relevant
    public RepairStatus? OldStatus { get; set; }
    public RepairStatus? NewStatus { get; set; }

    // Optional: JSON snapshot of full entity state
    public string? ChangeDetails { get; set; }
    public string? Notes { get; set; }
  }

  public enum RepairAuditAction
  {
    Created,
    Modified,
    StatusAdvanced,
    Cancelled,
    Delivered
  }
}