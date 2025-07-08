using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Lesson
{
    public class LessonCreateDto
    {
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El profesor es obligatorio")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "La cancha es obligatoria")]
        public int CourtId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage = "El cupo máximo es obligatorio")]
        [Range(1, 50, ErrorMessage = "El cupo máximo debe estar entre 1 y 50 alumnos")]
        public int MaxCapacity { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "El tipo de clase no puede exceder los 100 caracteres")]
        public string? ClassType { get; set; }

        // Campos para clases recurrentes
        public bool IsRecurrent { get; set; } = false;

        public DateTime? RecurrenceEndDate { get; set; }

        [StringLength(20, ErrorMessage = "El patrón de recurrencia no puede exceder los 20 caracteres")]
        public string? RecurrencePattern { get; set; } // "daily", "weekly", "monthly"

        public int? RecurrenceInterval { get; set; } = 1; // cada X días/semanas/meses

        // Para recurrencia semanal: días de la semana
        public List<DayOfWeek>? WeeklyDays { get; set; }
    }
} 