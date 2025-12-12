using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Product
{
    public class UpdateProductDto
    {
        [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int? Stock { get; set; }

        [Url(ErrorMessage = "La URL de la imagen no es válida")]
        public string? ImageUrl { get; set; }

        public bool? IsActive { get; set; }

        public int? CategoryId { get; set; }
    }
}

