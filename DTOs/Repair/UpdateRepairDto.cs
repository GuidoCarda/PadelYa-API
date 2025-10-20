using padelya_api.Models.Repair;

namespace padelya_api.DTOs.Repair
{
  public class UpdateRepairDto
  {
    public string? CustomerName { get; set; } = String.Empty;
    public string? CustomerEmail { get; set; } = String.Empty;
    public string? CustomerPhone { get; set; } = String.Empty;
    public decimal? Price { get; set; }
    public string? DamageDescription { get; set; } = String.Empty;
    public string? RepairNotes { get; set; } = String.Empty;
    public Racket? Racket { get; set; }
  }
}