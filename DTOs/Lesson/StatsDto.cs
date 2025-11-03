using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Lesson
{
    public class StatsDto
    {
        public int Id { get; set; }
        public float Drive { get; set; }
        public float Backhand { get; set; }
        public float Smash { get; set; }
        public float Serve { get; set; }
        public float Vibora { get; set; }
        public float Bandeja { get; set; }
        public string? Observations { get; set; }
        public string? Milestones { get; set; }
        public int PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerSurname { get; set; }
        public int? LessonId { get; set; }
        public DateTime? LessonDate { get; set; }
        public DateTime RecordedAt { get; set; }
        public int? RecordedByTeacherId { get; set; }
        public string? TeacherName { get; set; }
    }

    public class StatsCreateDto
    {
        [Required]
        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float Drive { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float Backhand { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float Smash { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float Serve { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float Vibora { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float Bandeja { get; set; }

        [StringLength(1000)]
        public string? Observations { get; set; }

        [StringLength(500)]
        public string? Milestones { get; set; }

        [Required]
        public int PlayerId { get; set; }

        public int? LessonId { get; set; }
    }

    public class StatsUpdateDto
    {
        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float? Drive { get; set; }

        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float? Backhand { get; set; }

        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float? Smash { get; set; }

        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float? Serve { get; set; }

        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float? Vibora { get; set; }

        [Range(0, 10, ErrorMessage = "El valor debe estar entre 0 y 10")]
        public float? Bandeja { get; set; }

        [StringLength(1000)]
        public string? Observations { get; set; }

        [StringLength(500)]
        public string? Milestones { get; set; }
    }
}

