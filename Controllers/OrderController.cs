using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Order;
using padelya_api.DTOs.Report;
using padelya_api.Services.Order;
using System.Security.Claims;
using padelya_api.Attributes;
using padelya_api.Constants;

namespace padelya_api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class OrderController(OrderService orderService) : ControllerBase
    {
        private readonly OrderService _orderService = orderService;

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            try
            {
                var (order, preferenceId, initPoint) = await _orderService.CreateOrderAsync(checkoutDto);
                return Ok(new { orderId = order.Id, preferenceId, initPoint });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<List<OrderDto>>> GetMyOrders()
        {
            try
            {
                var personIdClaim = User.Claims.FirstOrDefault(c => c.Type == "person_id");
                if (personIdClaim == null || !int.TryParse(personIdClaim.Value, out int personId))
                {
                    return Unauthorized("No se pudo identificar al usuario");
                }

                var orders = await _orderService.GetOrdersByPersonIdAsync(personId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("admin/all")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrador,Admin")] // Soporte para ambos nombres comunes, o usar Policy
        public async Task<ActionResult<List<OrderAdminDto>>> GetAllOrders()
        {
            try 
            {
                // Alternativa de seguridad: Verificar permiso específico si existe política
                // if (!User.HasClaim("permissions", Permissions.Order.ViewAll)) return Forbid();

                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpGet("admin/{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrador,Admin")]
        public async Task<ActionResult<OrderAdminDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null) return NotFound(new { message = "Pedido no encontrado" });
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("admin/{id}/status")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrador,Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "user_id");
                int? userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out int uid) ? uid : null;

                var result = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status, userId, "Status updated by admin via API");
                if (!result) return NotFound(new { message = "Pedido no encontrado" });
                
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("admin/report")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrador,Admin")]
        public async Task<ActionResult<ReportEcommerceDto>> GetEcommerceReport([FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                if (!DateTime.TryParseExact(startDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedStartDate))
                {
                    return BadRequest(new { message = "El formato de fecha inicial debe ser YYYY-MM-DD" });
                }

                if (!DateTime.TryParseExact(endDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedEndDate))
                {
                    return BadRequest(new { message = "El formato de fecha final debe ser YYYY-MM-DD" });
                }

                if (parsedStartDate > parsedEndDate)
                {
                    return BadRequest(new { message = "La fecha inicial no puede ser mayor a la fecha final" });
                }

                var report = await _orderService.GetEcommerceReportAsync(parsedStartDate, parsedEndDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
