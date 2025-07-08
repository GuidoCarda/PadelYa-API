using System.ComponentModel.DataAnnotations;
using padelya_api.models;

namespace padelya_api.DTOs.Lesson
{
    public class LessonResponseDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ClassType { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentEnrollments { get; set; }
        public bool IsAvailable => CurrentEnrollments < MaxCapacity;

        // Información del slot de la cancha
        public int CourtSlotId { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        // Información de la cancha
        public int CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;

        // Información del profesor
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string? TeacherTitle { get; set; }

        // Información de inscripciones
        public List<LessonEnrollmentDto> Enrollments { get; set; } = new();

        // Fechas de auditoría
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class LessonEnrollmentDto
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; }
        
        // Información del estudiante
        public int PersonId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCategory { get; set; } = string.Empty;

        // Información del pago
        public bool IsPaid { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
} 