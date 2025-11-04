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
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineService _routineService;

        public RoutineController(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        /// <summary>
        /// Crear una nueva rutina
        /// </summary>
        [HttpPost]
        [RequirePermission("lesson:assign_user")] // Solo profesores
        public async Task<IActionResult> CreateRoutine([FromBody] RoutineCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var teacherId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var result = await _routineService.CreateRoutineAsync(createDto, teacherId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Actualizar una rutina
        /// </summary>
        [HttpPut("{routineId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> UpdateRoutine(int routineId, [FromBody] RoutineUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var teacherId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var result = await _routineService.UpdateRoutineAsync(routineId, updateDto, teacherId);

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
        /// Obtener rutina por ID
        /// </summary>
        [HttpGet("{routineId}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetRoutineById(int routineId)
        {
            var result = await _routineService.GetRoutineByIdAsync(routineId);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Obtener rutinas de un profesor
        /// </summary>
        [HttpGet("teacher/{teacherId}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetRoutinesByTeacher(int teacherId)
        {
            var result = await _routineService.GetRoutinesByTeacherAsync(teacherId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener rutinas de un jugador
        /// </summary>
        [HttpGet("player/{playerId}")]
        [RequirePermission("lesson:view_own")]
        public async Task<IActionResult> GetRoutinesByPlayer(int playerId)
        {
            var result = await _routineService.GetRoutinesByPlayerAsync(playerId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Eliminar una rutina
        /// </summary>
        [HttpDelete("{routineId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> DeleteRoutine(int routineId)
        {
            var teacherId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var result = await _routineService.DeleteRoutineAsync(routineId, teacherId);

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

