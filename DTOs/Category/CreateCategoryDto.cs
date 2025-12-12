using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }
    }
}

