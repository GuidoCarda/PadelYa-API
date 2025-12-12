using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Category
{
    public class UpdateCategoryDto
    {
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}

