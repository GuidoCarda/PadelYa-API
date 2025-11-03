using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.Attributes;
using padelya_api.DTOs.Lesson;
using padelya_api.Services;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassTypeController : ControllerBase
    {
        private readonly IClassTypeService _classTypeService;

        public ClassTypeController(IClassTypeService classTypeService)
        {
            _classTypeService = classTypeService;
        }

        /// <summary>
        /// Obtener todos los tipos de clase
        /// </summary>
        [HttpGet]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _classTypeService.GetAllAsync();
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener tipo de clase por ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _classTypeService.GetByIdAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }

            if (result.ErrorCode == "NOT_FOUND")
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Crear un nuevo tipo de clase
        /// </summary>
        [HttpPost]
        [RequirePermission("lesson:create")]
        public async Task<IActionResult> Create([FromBody] ClassTypeCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var result = await _classTypeService.CreateAsync(createDto);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Actualizar un tipo de clase existente
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermission("lesson:edit")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassTypeUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("El ID del parámetro no coincide con el ID del DTO");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var result = await _classTypeService.UpdateAsync(updateDto);
            
            if (result.Success)
            {
                return Ok(result);
            }

            if (result.ErrorCode == "NOT_FOUND")
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Eliminar un tipo de clase
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermission("lesson:delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _classTypeService.DeleteAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }

            if (result.ErrorCode == "NOT_FOUND")
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }
    }
}

