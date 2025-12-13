using padelya_api.Constants;

namespace padelya_api.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? PreferenceId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
