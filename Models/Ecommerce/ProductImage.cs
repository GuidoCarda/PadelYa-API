using System;

namespace padelya_api.Models.Ecommerce
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } // 1, 2, 3 para ordenar
        public bool IsPrimary { get; set; } = false; // La imagen principal
        public DateTime CreatedAt { get; set; }

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation property
        public Product Product { get; set; } = null!;
    }
}

