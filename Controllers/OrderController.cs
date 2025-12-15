using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Order;
using padelya_api.DTOs.Order;
using padelya_api.Services.Order;
using System.Security.Claims;

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
                var result = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
                if (!result) return NotFound(new { message = "Pedido no encontrado" });
                
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
