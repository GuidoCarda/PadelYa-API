using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Product;
using padelya_api.Models.Ecommerce;

namespace padelya_api.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly PadelYaDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(PadelYaDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductListDto>> GetAllProductsAsync(bool includeInactive = false)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(p => p.IsActive);
                }

                var products = await query
                    .Include(p => p.Images)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return products.Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.PrimaryImageUrl,
                    Images = p.Images.OrderBy(i => i.DisplayOrder).Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        DisplayOrder = i.DisplayOrder,
                        IsPrimary = i.IsPrimary
                    }).ToList(),
                    IsActive = p.IsActive,
                    CategoryName = p.Category.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos");
                throw;
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Images)
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return null;
                }

                return new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageUrl = product.PrimaryImageUrl,
                    Images = product.Images.OrderBy(i => i.DisplayOrder).Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        DisplayOrder = i.DisplayOrder,
                        IsPrimary = i.IsPrimary
                    }).ToList(),
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {ProductId}", id);
                throw;
            }
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            try
            {
                // Verificar que la categoría existe
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == createDto.CategoryId && c.IsActive);

                if (!categoryExists)
                {
                    throw new InvalidOperationException("La categoría especificada no existe o está inactiva");
                }

                var product = new padelya_api.Models.Ecommerce.Product
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Price = createDto.Price,
                    Stock = createDto.Stock,
                    ImageUrl = createDto.ImageUrl,
                    CategoryId = createDto.CategoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto creado exitosamente: {ProductName} (ID: {ProductId})", 
                    product.Name, product.Id);

                // Cargar la categoría y las imágenes para retornar el DTO completo
                await _context.Entry(product)
                    .Reference(p => p.Category)
                    .LoadAsync();

                await _context.Entry(product)
                    .Collection(p => p.Images)
                    .LoadAsync();

                return new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageUrl = product.PrimaryImageUrl,
                    Images = product.Images.OrderBy(i => i.DisplayOrder).Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        DisplayOrder = i.DisplayOrder,
                        IsPrimary = i.IsPrimary
                    }).ToList(),
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto");
                throw;
            }
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return null;
                }

                // Si se está cambiando la categoría, verificar que existe
                if (updateDto.CategoryId.HasValue && updateDto.CategoryId.Value != product.CategoryId)
                {
                    var categoryExists = await _context.Categories
                        .AnyAsync(c => c.Id == updateDto.CategoryId.Value && c.IsActive);

                    if (!categoryExists)
                    {
                        throw new InvalidOperationException("La categoría especificada no existe o está inactiva");
                    }

                    product.CategoryId = updateDto.CategoryId.Value;
                }

                // Actualizar solo los campos proporcionados
                if (updateDto.Name != null)
                    product.Name = updateDto.Name;

                if (updateDto.Description != null)
                    product.Description = updateDto.Description;

                if (updateDto.Price.HasValue)
                    product.Price = updateDto.Price.Value;

                if (updateDto.Stock.HasValue)
                    product.Stock = updateDto.Stock.Value;

                if (updateDto.ImageUrl != null)
                    product.ImageUrl = updateDto.ImageUrl;

                if (updateDto.IsActive.HasValue)
                    product.IsActive = updateDto.IsActive.Value;

                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto actualizado exitosamente: {ProductName} (ID: {ProductId})", 
                    product.Name, product.Id);

                // Recargar la categoría y las imágenes por si cambiaron
                await _context.Entry(product)
                    .Reference(p => p.Category)
                    .LoadAsync();

                await _context.Entry(product)
                    .Collection(p => p.Images)
                    .LoadAsync();

                return new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageUrl = product.PrimaryImageUrl,
                    Images = product.Images.OrderBy(i => i.DisplayOrder).Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        DisplayOrder = i.DisplayOrder,
                        IsPrimary = i.IsPrimary
                    }).ToList(),
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto con ID {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return false;
                }

                // Verificar si el producto tiene órdenes asociadas
                var hasOrders = await _context.OrderItems
                    .AnyAsync(oi => oi.ProductId == id);

                if (hasOrders)
                {
                    throw new InvalidOperationException(
                        "No se puede eliminar el producto porque tiene órdenes asociadas. " +
                        "Considere marcarlo como inactivo en su lugar.");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto eliminado exitosamente: {ProductName} (ID: {ProductId})", 
                    product.Name, product.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        IsActive = c.IsActive
                    })
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de categorías");
                throw;
            }
        }
    }
}

