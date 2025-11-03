using System.ComponentModel.DataAnnotations;
using padelya_api.Models.Lesson;

namespace padelya_api.DTOs.Lesson
{
    public class LessonAttendanceDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int PersonId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentSurname { get; set; } = string.Empty;
        public AttendanceStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime RecordedAt { get; set; }
        public int? RecordedByTeacherId { get; set; }
    }

    public class LessonAttendanceCreateDto
    {
        [Required]
        public int LessonId { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class LessonAttendanceBulkDto
    {
        [Required]
        public int LessonId { get; set; }

        [Required]
        public List<LessonAttendanceCreateDto> Attendances { get; set; } = new();
    }
}

