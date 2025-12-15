using padelya_api.Constants;

namespace padelya_api.DTOs.Order
{
    public class OrderAdminDto : OrderDto
    {
        public int PersonId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerSurname { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
