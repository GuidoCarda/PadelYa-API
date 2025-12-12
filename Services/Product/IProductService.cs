using padelya_api.DTOs.Product;

namespace padelya_api.Services.Product
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListDto>> GetAllProductsAsync(bool includeInactive = false);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    }
}

