using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Order;
using padelya_api.Models.Ecommerce;

namespace padelya_api.Services.Order
{
    public class OrderService(
        PadelYaDbContext context,
        IConfiguration configuration)
    {
        private readonly PadelYaDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<List<OrderDto>> GetOrdersByPersonIdAsync(int personId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.PersonId == personId && o.Status != OrderStatus.Draft)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PreferenceId = o.PreferenceId,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        ProductImage = oi.Product.ImageUrl,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal
                    }).ToList()
                })
                .ToListAsync();

            return orders;
        }

        public async Task<(Models.Ecommerce.Order Order, string PreferenceId, string InitPoint)> CreateOrderAsync(CheckoutDto checkoutDto)
        {
            // 1. Validate stock and calculate total
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in checkoutDto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId) 
                    ?? throw new Exception($"Producto {item.ProductId} no encontrado");

                if (!product.IsActive)
                    throw new Exception($"Producto {product.Name} no está disponible");

                if (product.Stock < item.Quantity)
                    throw new Exception($"Stock insuficiente para {product.Name}");

                var subtotal = product.Price * item.Quantity;
                totalAmount += subtotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = subtotal
                });
            }

            // 2. Create Order
            var order = new Models.Ecommerce.Order
            {
                PersonId = checkoutDto.PersonId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Draft,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3. Create Mercado Pago Preference
            var (preferenceId, initPoint) = await CreatePreferenceAsync(order, orderItems);
            
            order.PreferenceId = preferenceId;
            await _context.SaveChangesAsync();

            return (order, preferenceId, initPoint);
        }

        private async Task<(string PreferenceId, string InitPoint)> CreatePreferenceAsync(Models.Ecommerce.Order order, List<OrderItem> items)
        {
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];

            var preferenceRequest = new PreferenceRequest
            {
                Items = items.Select(i => new PreferenceItemRequest
                {
                    Id = i.ProductId.ToString(),
                    Title = _context.Products.Find(i.ProductId)?.Name ?? "Producto",
                    Quantity = i.Quantity,
                    CurrencyId = "ARS",
                    UnitPrice = i.UnitPrice
                }).ToList(),
                Payer = new PreferencePayerRequest
                {
                     // Get user email if possible, for now simplified
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = $"{_configuration["AppSettings:FrontBaseUrl"]}/orders/success",
                    Failure = $"{_configuration["AppSettings:FrontBaseUrl"]}/orders/failure",
                    Pending = $"{_configuration["AppSettings:FrontBaseUrl"]}/orders/pending"
                },
                Metadata = new Dictionary<string, object>
                {
                    ["order_id"] = order.Id.ToString(),
                    ["person_id"] = order.PersonId.ToString()
                },
                NotificationUrl = $"{_configuration["AppSettings:ApiBaseUrl"]}/api/payments/webhook",
                AutoReturn = "approved",
                ExternalReference = $"order_{order.Id}",
                StatementDescriptor = "PadelYa Store"
            };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(preferenceRequest);

            return (preference.Id, preference.InitPoint);
        }
    }
}
