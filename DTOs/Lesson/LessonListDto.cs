namespace padelya_api.DTOs.Lesson
{
    public class LessonListDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string? ClassType { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentEnrollments { get; set; }
        public bool IsAvailable => CurrentEnrollments < MaxCapacity;

        // Información del slot
        public DateTime Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        // Información básica de la cancha
        public string CourtName { get; set; } = string.Empty;

        // Información básica del profesor
        public string TeacherName { get; set; } = string.Empty;

        // Estado de la clase
        public string Status { get; set; } = "Programada"; // Programada, En Curso, Finalizada, Cancelada
    }

    public class LessonFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TeacherId { get; set; }
        public int? CourtId { get; set; }
        public string? ClassType { get; set; }
        public bool? AvailableOnly { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
} 