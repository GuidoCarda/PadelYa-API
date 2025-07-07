using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Tournament
{
    public class CreateTournamentDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public required string Category { get; set; }

        [Required(ErrorMessage = "Los cupos son obligatorios.")]
        [Range(1, int.MaxValue, ErrorMessage = "Los cupos deben ser al menos 1.")]
        public int Quota { get; set; }

        [Required(ErrorMessage = "El precio de inscripción es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        public decimal EnrollmentPrice { get; set; }

        [Required]
        public DateTime EnrollmentStartDate { get; set; }

        [Required]
        public DateTime EnrollmentEndDate { get; set; }

        [Required]
        public DateTime TournamentStartDate { get; set; }

        [Required]
        public DateTime TournamentEndDate { get; set; }
    }
}
