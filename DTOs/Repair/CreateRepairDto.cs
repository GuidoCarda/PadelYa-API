using padelya_api.Constants;
using padelya_api.Models.Repair;

namespace padelya_api.DTOs.Repair
{
  public class CreateRepairDto
  {
    public string CustomerName { get; set; } = String.Empty;
    public string CustomerEmail { get; set; } = String.Empty;
    public string CustomerPhone { get; set; } = String.Empty;
    public int? PersonId { get; set; }
    public decimal Price { get; set; }
    public Racket Racket { get; set; }
    public string DamageDescription { get; set; } = String.Empty;
    public string? RepairNotes { get; set; } = String.Empty;
    public DateTime EstimatedCompletionTime { get; set; }
  }
}