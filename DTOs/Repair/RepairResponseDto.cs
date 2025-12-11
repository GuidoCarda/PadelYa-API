using padelya_api.Models;
using padelya_api.Models.Repair;
using padelya_api.Models.Repair.States;
using PaymentModel = padelya_api.Models.Payment;

namespace padelya_api.DTOs.Repair
{
  public class RepairResponseDto
  {
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? EstimatedCompletionTime { get; set; }
    public decimal Price { get; set; }
    public string DamageDescription { get; set; } = string.Empty;
    public string RepairNotes { get; set; } = string.Empty;
    public RepairStatus Status { get; set; }

    // Navigation data
    public int RacketId { get; set; }
    public Racket? Racket { get; set; }
    public int PersonId { get; set; }
    public Person? Person { get; set; }
    public int? PaymentId { get; set; }
    public PaymentModel? Payment { get; set; }

    // Audit-related fields
    public string? CancellationReason { get; set; }
    public List<StatusHistoryDto> StatusHistory { get; set; } = new();
  }

  public class StatusHistoryDto
  {
    public RepairStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
    public RepairAuditAction Action { get; set; }
  }
}

