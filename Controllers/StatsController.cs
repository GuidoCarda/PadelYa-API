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
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        /// <summary>
        /// Registrar progreso de un alumno
        /// </summary>
        [HttpPost]
        [RequirePermission("lesson:assign_user")] // Solo profesores
        public async Task<IActionResult> CreateStats([FromBody] StatsCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var teacherId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var result = await _statsService.CreateStatsAsync(createDto, teacherId);

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
        /// Actualizar progreso de un alumno
        /// </summary>
        [HttpPut("{statsId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> UpdateStats(int statsId, [FromBody] StatsUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var teacherId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var result = await _statsService.UpdateStatsAsync(statsId, updateDto, teacherId);

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
        /// Obtener progreso por ID
        /// </summary>
        [HttpGet("{statsId}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetStatsById(int statsId)
        {
            var result = await _statsService.GetStatsByIdAsync(statsId);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Obtener progreso de un jugador
        /// </summary>
        [HttpGet("player/{playerId}")]
        [RequirePermission("lesson:view_own")]
        public async Task<IActionResult> GetStatsByPlayer(int playerId)
        {
            var result = await _statsService.GetStatsByPlayerAsync(playerId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener progreso de una clase
        /// </summary>
        [HttpGet("lesson/{lessonId}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetStatsByLesson(int lessonId)
        {
            var result = await _statsService.GetStatsByLessonAsync(lessonId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Eliminar registro de progreso
        /// </summary>
        [HttpDelete("{statsId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> DeleteStats(int statsId)
        {
            var teacherId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var result = await _statsService.DeleteStatsAsync(statsId, teacherId);

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

