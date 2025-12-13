using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace padelya_api.DTOs.Order
{
    public class CheckoutDto
    {
        [Required]
        public int PersonId { get; set; }

        [Required]
        public List<CheckoutItemDto> Items { get; set; } = new();
    }

    public class CheckoutItemDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
