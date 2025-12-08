using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Lesson
{
    public class RoutineDto
    {
        public int Id { get; set; }
        public TimeSpan Duration { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public List<int> PlayerIds { get; set; } = new();
        public List<string>? PlayerNames { get; set; }
        public List<int> ExerciseIds { get; set; } = new();
        public List<ExerciseDto>? Exercises { get; set; }
    }

    public class ExerciseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class ExerciseCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
    }

    public class ExerciseUpdateDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }
    }

    public class RoutineCreateDto
    {
        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public List<int> PlayerIds { get; set; } = new();

        [Required]
        public List<int> ExerciseIds { get; set; } = new();
    }

    public class RoutineUpdateDto
    {
        public TimeSpan? Duration { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public List<int>? PlayerIds { get; set; }

        public List<int>? ExerciseIds { get; set; }
    }

    public class LessonRoutineAssignmentDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? PersonSurname { get; set; }
        public int RoutineId { get; set; }
        public string? RoutineCategory { get; set; }
        public string? RoutineDescription { get; set; }
        public DateTime AssignedAt { get; set; }
    }

    public class LessonRoutineAssignmentBulkDto
    {
        [Required]
        public int LessonId { get; set; }

        [Required]
        public List<LessonRoutineAssignmentCreateDto> Assignments { get; set; } = new();
    }

    public class LessonRoutineAssignmentCreateDto
    {
        [Required]
        public int PersonId { get; set; }

        [Required]
        public int RoutineId { get; set; }
    }
}

