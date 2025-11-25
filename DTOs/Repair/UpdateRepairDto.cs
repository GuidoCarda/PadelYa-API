namespace padelya_api.DTOs.Repair
{
  public class UpdateRepairDto
  {
    // Repair details
    public decimal? Price { get; set; }
    public string? DamageDescription { get; set; }
    public string? RepairNotes { get; set; }
    public DateTime? EstimatedCompletionTime { get; set; }
    public string? Status { get; set; }

    // Racket details
    public UpdateRacketDto? Racket { get; set; }
  }

  public class UpdateRacketDto
  {
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialCode { get; set; }
  }
}