using padelya_api.Models.Ecommerce;

namespace padelya_api.Services.Ecommerce
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task<Cart> AddItemToCartAsync(int userId, int productId, int quantity);
        Task<Cart> RemoveItemFromCartAsync(int userId, int productId);
        Task<Cart> UpdateItemQuantityAsync(int userId, int productId, int quantity);
        Task ClearCartAsync(int userId);
    }
}
