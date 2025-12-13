using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.Models.Ecommerce;
using padelya_api.Services.Ecommerce;
using System.Security.Claims;

namespace padelya_api.Controllers.Ecommerce
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("user_id")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            // Fallback for testing/dev if needed, or throw
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        [HttpGet]
        public async Task<ActionResult<Cart>> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<Cart>> AddItem([FromBody] AddCartItemDto dto)
        {
            var userId = GetUserId();
            var cart = await _cartService.AddItemToCartAsync(userId, dto.ProductId, dto.Quantity);
            return Ok(cart);
        }

        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<Cart>> RemoveItem(int productId)
        {
            var userId = GetUserId();
            var cart = await _cartService.RemoveItemFromCartAsync(userId, productId);
            return Ok(cart);
        }

        [HttpPut("items")]
        public async Task<ActionResult<Cart>> UpdateItemQuantity([FromBody] UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdateItemQuantityAsync(userId, dto.ProductId, dto.Quantity);
            return Ok(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }

    public class AddCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
