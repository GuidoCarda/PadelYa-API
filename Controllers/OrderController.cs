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
    }
}
