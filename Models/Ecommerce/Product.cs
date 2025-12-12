using System;
using System.Collections.Generic;
using System.Linq;

namespace padelya_api.Models.Ecommerce
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; } // Deprecated - mantener por compatibilidad
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }

        // Navigation properties
        public Category Category { get; set; } = null!;
        public List<ProductImage> Images { get; set; } = new();

        // Helper property para obtener la imagen principal
        public string? PrimaryImageUrl => Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl 
                                          ?? Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.ImageUrl
                                          ?? ImageUrl;
    }
}

