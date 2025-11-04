using padelya_api.Models;
using padelya_api.models;

namespace padelya_api.Models.Class
{
    public class Stats
    {
        public int Id { get; set; }

        public float Drive { get; set; }
        public float Backhand { get; set; }
        public float Smash { get; set; }
        public float Serve { get; set; }
        public float Vibora { get; set; }
        public float Bandeja { get; set; }

        // Observaciones y notas del profesor
        public string? Observations { get; set; }
        
        // Hitos o logros alcanzados
        public string? Milestones { get; set; }

        // Navigation property for Player (one player per stats entry)
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        // Navigation property for Lesson (opcional, puede ser progreso general)
        public int? LessonId { get; set; }
        public padelya_api.models.Lesson? Lesson { get; set; }

        // Fecha de registro
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        
        // Profesor que registró el progreso
        public int? RecordedByTeacherId { get; set; }
        public Teacher? RecordedByTeacher { get; set; }
    }
}