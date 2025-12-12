using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Category;
using padelya_api.Services.Category;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las categorías
        /// </summary>
        /// <param name="includeInactive">Incluir categorías inactivas</param>
        /// <returns>Lista de categorías</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories(
            [FromQuery] bool includeInactive = false)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync(includeInactive);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de categorías");
                return StatusCode(500, ResponseMessage.Error("Error al obtener las categorías"));
            }
        }

        /// <summary>
        /// Obtiene una categoría específica por su ID
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Detalles de la categoría</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound(ResponseMessage.Error("Categoría no encontrada"));
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría con ID {CategoryId}", id);
                return StatusCode(500, ResponseMessage.Error("Error al obtener la categoría"));
            }
        }

        /// <summary>
        /// Crea una nueva categoría
        /// RF13 - Agregar nueva categoría
        /// </summary>
        /// <param name="createDto">Datos de la categoría a crear</param>
        /// <returns>Categoría creada</returns>
        [HttpPost]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("category:create")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = await _categoryService.CreateCategoryAsync(createDto);
                return CreatedAtAction(
                    nameof(GetCategoryById),
                    new { id = category.Id },
                    category
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseMessage.Error(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoría");
                return StatusCode(500, ResponseMessage.Error("Error al crear la categoría"));
            }
        }

        /// <summary>
        /// Actualiza una categoría existente
        /// RF14 - Modificar categoría existente
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <param name="updateDto">Datos a actualizar</param>
        /// <returns>Categoría actualizada</returns>
        [HttpPut("{id}")]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("category:edit")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = await _categoryService.UpdateCategoryAsync(id, updateDto);

                if (category == null)
                {
                    return NotFound(ResponseMessage.Error("Categoría no encontrada"));
                }

                return Ok(category);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseMessage.Error(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría con ID {CategoryId}", id);
                return StatusCode(500, ResponseMessage.Error("Error al actualizar la categoría"));
            }
        }

        /// <summary>
        /// Elimina una categoría
        /// RF15 - Eliminar categoría existente
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("category:delete")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (!result)
                {
                    return NotFound(ResponseMessage.Error("Categoría no encontrada"));
                }

                return Ok(ResponseMessage.SuccessMessage("Categoría eliminada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseMessage.Error(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría con ID {CategoryId}", id);
                return StatusCode(500, ResponseMessage.Error("Error al eliminar la categoría"));
            }
        }
    }
}

