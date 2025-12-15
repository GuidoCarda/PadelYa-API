using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Order;
using padelya_api.Models.Ecommerce;
using padelya_api.DTOs.Report;

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
                .ThenInclude(p => p.Images)
                .Where(o => o.PersonId == personId && o.Status != OrderStatus.Draft)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    PreferenceId = o.PreferenceId,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        ProductImage = oi.Product.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault() 
                                       ?? oi.Product.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault() 
                                       ?? oi.Product.ImageUrl,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal
                    }).ToList()
                })
                .ToListAsync();

            return orders;
        }

        public async Task<List<OrderAdminDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Person)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Images)
                .Where(o => o.Status != OrderStatus.Draft)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderAdminDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    PreferenceId = o.PreferenceId,
                    PersonId = o.PersonId,
                    // Intentamos obtener datos del usuario desde Person. 
                    // Nota: Person puede no tener User si es una relación diferente, pero User tiene PersonId.
                    // Si Person tiene Name/Surname/Email directos, usarlos.
                    CustomerName = o.Person.Name, 
                    CustomerSurname = o.Person.Surname,
                    CustomerEmail = o.Person.Email,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        ProductImage = oi.Product.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault() 
                                       ?? oi.Product.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault() 
                                       ?? oi.Product.ImageUrl,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal
                    }).ToList()
                })
                .ToListAsync();

            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, int? userId = null, string? changeDetails = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            
            if (order == null)
                return false;

            var oldStatus = order.Status;
            
            if (oldStatus == newStatus)
                return true;

            order.UpdateStatus(newStatus);

            await LogAuditAsync(order, userId, "UpdateStatus", oldStatus.ToString(), newStatus.ToString(), changeDetails);

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task LogAuditAsync(Models.Ecommerce.Order order, int? userId, string action, string? oldStatus = null, string? newStatus = null, string? changeDetails = null)
        {
            var audit = new OrderAuditLog
            {
                OrderId = order.Id,
                UserId = userId,
                Action = action,
                TimeStamp = DateTime.UtcNow,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangeDetails = changeDetails
            };

            _context.OrderAuditLogs.Add(audit);
            // SaveChangesAsync is usually called by the caller (e.g. UpdateOrderStatusAsync), 
            // but for CreateOrderAsync we might want to ensure it's saved.
            // Since we are adding to the context, it will be saved when _context.SaveChangesAsync() is called.
        }



        public async Task<OrderAdminDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Person)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            return new OrderAdminDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                PreferenceId = order.PreferenceId,
                PersonId = order.PersonId,
                CustomerName = order.Person.Name,
                CustomerSurname = order.Person.Surname,
                CustomerEmail = order.Person.Email,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImage = oi.Product.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault() 
                                   ?? oi.Product.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault() 
                                   ?? oi.Product.ImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                }).ToList()
            };
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
                PaymentStatus = padelya_api.Constants.PaymentStatus.Pending,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Audit Creation
            await LogAuditAsync(order, checkoutDto.PersonId, "Create", null, "Draft", "Order created via checkout");
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

        public async Task<ReportEcommerceDto> GetEcommerceReportAsync(DateTime startDate, DateTime endDate)
        {
            // Normalize dates
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddTicks(-1);

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && o.Status != OrderStatus.Draft)
                .ToListAsync();

            var report = new ReportEcommerceDto();

            // 1. General Stats
            // Only consider completed/valid orders for revenue? 
            // Usually for reports we might want to see everything except maybe Drafts. 
            // Let's exclude Cancelled from Revenue calculation but maybe keep them for status distribution.
            var validOrders = orders.Where(o => o.Status != OrderStatus.Cancelled).ToList();

            report.Statistics.TotalRevenue = validOrders.Sum(o => o.TotalAmount);
            report.Statistics.TotalOrders = orders.Count; // Total count including cancelled? Or just valid? Let's say Total placed orders.
            report.Statistics.AverageTicket = report.Statistics.TotalOrders > 0 
                ? report.Statistics.TotalRevenue / validOrders.Count // Avg of valid orders
                : 0;

            if (validOrders.Count > 0)
            {
                 report.Statistics.AverageTicket = report.Statistics.TotalRevenue / validOrders.Count;
            }

            // 2. Daily Sales (Revenue & Count)
            // Group all orders by date
            var groupedByDate = orders
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailySalesDto
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    // Revenue only from valid orders
                    Revenue = g.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Fill missing dates? Optional, but good for charts.
            // For now let's return what we have, frontend can handle gaps or we can fill here.
            report.DailySales = groupedByDate;

            // 3. Status Distribution
            var totalOrders = orders.Count;
            if (totalOrders > 0)
            {
                report.StatusDistribution = orders
                    .GroupBy(o => o.Status)
                    .Select(g => new StatusDistributionDto
                    {
                        Status = g.Key.ToString(),
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / totalOrders * 100, 1)
                    })
                    .ToList();
            }

            // 4. Top Products (by Quantity Sold or Revenue)
            // We need to flatten ALL OrderItems from valid orders
            var allItems = validOrders.SelectMany(o => o.OrderItems);

            report.TopProducts = allItems
                .GroupBy(i => i.ProductId)
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name, // Assuming Product is included
                    QuantitySold = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.Subtotal)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToList();

            return report;
        }
    }
}
