using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.Attributes;
using padelya_api.DTOs.Lesson;
using padelya_api.Services;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExerciseController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        /// <summary>
        /// Obtener todos los ejercicios
        /// </summary>
        [HttpGet]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetAllExercises()
        {
            var result = await _exerciseService.GetAllExercisesAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener un ejercicio por ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetExerciseById(int id)
        {
            var result = await _exerciseService.GetExerciseByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Message != null && result.Message.Contains("no encontrado"))
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Crear un nuevo ejercicio
        /// </summary>
        [HttpPost]
        [RequirePermission("lesson:assign_user")] // Solo profesores pueden crear ejercicios
        public async Task<IActionResult> CreateExercise([FromBody] ExerciseCreateDto createDto)
        {
            var result = await _exerciseService.CreateExerciseAsync(createDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Actualizar un ejercicio
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermission("lesson:assign_user")] // Solo profesores pueden actualizar ejercicios
        public async Task<IActionResult> UpdateExercise(int id, [FromBody] ExerciseUpdateDto updateDto)
        {
            var result = await _exerciseService.UpdateExerciseAsync(id, updateDto);

            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Message != null && result.Message.Contains("no encontrado"))
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Eliminar un ejercicio
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermission("lesson:assign_user")] // Solo profesores pueden eliminar ejercicios
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var result = await _exerciseService.DeleteExerciseAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Message != null && result.Message.Contains("no encontrado"))
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }
    }
}

