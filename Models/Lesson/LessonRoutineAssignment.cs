using System.ComponentModel.DataAnnotations;
using padelya_api.Models;
using padelya_api.Models.Class;

namespace padelya_api.Models.Lesson
{
    public class LessonRoutineAssignment
    {
        public int Id { get; set; }

        [Required]
        public int LessonId { get; set; }
        public padelya_api.models.Lesson Lesson { get; set; } = null!;

        [Required]
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        [Required]
        public int RoutineId { get; set; }
        public Routine Routine { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public int AssignedByTeacherId { get; set; }
        public Teacher AssignedByTeacher { get; set; } = null!;
    }
}

