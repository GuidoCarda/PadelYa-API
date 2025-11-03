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
}

