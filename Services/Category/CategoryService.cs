using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Category;

namespace padelya_api.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly PadelYaDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(PadelYaDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(bool includeInactive = false)
        {
            try
            {
                var query = _context.Categories.AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(c => c.IsActive);
                }

                var categories = await query
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                var result = new List<CategoryDto>();
                
                foreach (var category in categories)
                {
                    // Contar productos activos de forma segura
                    var productCount = await _context.Products
                        .Where(p => p.CategoryId == category.Id && p.IsActive)
                        .CountAsync();

                    result.Add(new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        IsActive = category.IsActive,
                        ProductCount = productCount
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de categorías", ex);
                throw;
            }
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return null;
                }

                // Contar productos activos de forma segura
                var productCount = await _context.Products
                    .Where(p => p.CategoryId == category.Id && p.IsActive)
                    .CountAsync();

                return new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    ProductCount = productCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría con ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            try
            {
                // Verificar que no exista una categoría con el mismo nombre
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == createDto.Name.ToLower());

                if (existingCategory != null)
                {
                    throw new InvalidOperationException($"Ya existe una categoría con el nombre '{createDto.Name}'");
                }

                var category = new Models.Ecommerce.Category
                {
                    Name = createDto.Name,
                    Description = createDto.Description ?? string.Empty,
                    IsActive = true
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoría creada exitosamente: {CategoryName} (ID: {CategoryId})",
                    category.Name, category.Id);

                return new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    ProductCount = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoría");
                throw;
            }
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return null;
                }

                // Si se está cambiando el nombre, verificar que no exista otro con el mismo nombre
                if (updateDto.Name != null && updateDto.Name != category.Name)
                {
                    var existingCategory = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name.ToLower() == updateDto.Name.ToLower() && c.Id != id);

                    if (existingCategory != null)
                    {
                        throw new InvalidOperationException($"Ya existe una categoría con el nombre '{updateDto.Name}'");
                    }

                    category.Name = updateDto.Name;
                }

                if (updateDto.Description != null)
                {
                    category.Description = updateDto.Description;
                }

                if (updateDto.IsActive.HasValue)
                {
                    category.IsActive = updateDto.IsActive.Value;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoría actualizada exitosamente: {CategoryName} (ID: {CategoryId})",
                    category.Name, category.Id);

                // Contar productos activos de forma segura
                var productCount = await _context.Products
                    .Where(p => p.CategoryId == category.Id && p.IsActive)
                    .CountAsync();

                return new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    ProductCount = productCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría con ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return false;
                }

                // RF15: Verificar que no tenga productos asociados
                var productCount = await _context.Products
                    .Where(p => p.CategoryId == id)
                    .CountAsync();

                if (productCount > 0)
                {
                    throw new InvalidOperationException(
                        $"No se puede eliminar la categoría '{category.Name}' porque tiene {productCount} producto(s) asociado(s). " +
                        "Elimine o reasigne los productos primero.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoría eliminada exitosamente: {CategoryName} (ID: {CategoryId})",
                    category.Name, category.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría con ID {CategoryId}", id);
                throw;
            }
        }
    }
}

