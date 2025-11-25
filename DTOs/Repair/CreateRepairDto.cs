using System.ComponentModel.DataAnnotations;
using padelya_api.Models.Repair;

namespace padelya_api.DTOs.Repair
{
  public class CreateRepairDto
  {
    [Required(ErrorMessage = "PersonId is required. Please select an existing user.")]
    public int PersonId { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Racket information is required.")]
    public Racket Racket { get; set; }

    [Required(ErrorMessage = "Damage description is required.")]
    [MinLength(1, ErrorMessage = "Damage description cannot be empty.")]
    public string DamageDescription { get; set; } = String.Empty;

    public string? RepairNotes { get; set; } = String.Empty;

    [Required(ErrorMessage = "Estimated completion time is required.")]
    public DateTime EstimatedCompletionTime { get; set; }
  }
}