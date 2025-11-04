using padelya_api.Models;
using padelya_api.Models.Class;
using System.ComponentModel.DataAnnotations;

namespace padelya_api.models
{
    public class Lesson
    {
        public int Id { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 50)]
        public int MaxCapacity { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ClassType { get; set; }

        // Estado manual de la clase (Programada, En Curso, Finalizada, Cancelada)
        // Si es null, se calcula automáticamente basándose en fechas
        [StringLength(50)]
        public string? Status { get; set; }

        // Campos de auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property for Teacher (one teacher per class)
        [Required]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        // Navigation property for Enrollments
        public List<LessonEnrollment> Enrollments { get; set; } = new();

        // Navigation property for Stats (1 to 4 reports per class)
        public List<Stats> Reports { get; set; } = new();

        // Navigation property for CourtSlot (One slot per class)
        [Required]
        public int CourtSlotId { get; set; }
        public CourtSlot CourtSlot { get; set; } = null!;

        // Propiedades calculadas
        public int CurrentEnrollments => Enrollments?.Count ?? 0;
        public bool IsAvailable => CurrentEnrollments < MaxCapacity;
        public bool HasStarted => CourtSlot?.Date.Add(CourtSlot.StartTime.ToTimeSpan()) <= DateTime.Now;
        public bool HasEnded => CourtSlot?.Date.Add(CourtSlot.EndTime.ToTimeSpan()) <= DateTime.Now;
    }
}