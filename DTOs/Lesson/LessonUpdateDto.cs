using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Lesson
{
    public class LessonUpdateDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal? Price { get; set; }

        public int? TeacherId { get; set; }

        public int? CourtId { get; set; }

        public DateTime? Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        [Range(1, 50, ErrorMessage = "El cupo máximo debe estar entre 1 y 50 alumnos")]
        public int? MaxCapacity { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "El tipo de clase no puede exceder los 100 caracteres")]
        public string? ClassType { get; set; }
    }
} 