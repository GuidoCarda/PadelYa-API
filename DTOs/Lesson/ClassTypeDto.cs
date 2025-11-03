using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Lesson
{
    public class ClassTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int LessonsCount { get; set; } // Cantidad de clases que usan este tipo
    }

    public class ClassTypeCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "El nivel no puede exceder los 50 caracteres")]
        public string? Level { get; set; }
    }

    public class ClassTypeUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "El nivel no puede exceder los 50 caracteres")]
        public string? Level { get; set; }
    }
}

