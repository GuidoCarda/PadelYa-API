using System.ComponentModel.DataAnnotations;

namespace padelya_api.Models.Lesson
{
    public class ClassType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Level { get; set; } // Ej: "Iniciación", "Intermedio", "Avanzado", "Competición", "Infantil"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property - clases que usan este tipo
        // Temporalmente comentado hasta crear la migración con ClassTypeId en Lesson
        // public List<padelya_api.models.Lesson> Lessons { get; set; } = new();
    }
}

