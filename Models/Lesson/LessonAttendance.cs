using System.ComponentModel.DataAnnotations;
using padelya_api.models;
using padelya_api.Models;

namespace padelya_api.Models.Lesson
{
    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Justified = 3
    }

    public class LessonAttendance
    {
        public int Id { get; set; }

        [Required]
        public int LessonId { get; set; }
        public padelya_api.models.Lesson Lesson { get; set; } = null!;

        [Required]
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        [Required]
        public AttendanceStatus Status { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; } // Notas adicionales del profesor

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public int? RecordedByTeacherId { get; set; } // ID del profesor que registró la asistencia
        public Teacher? RecordedByTeacher { get; set; }
    }
}

