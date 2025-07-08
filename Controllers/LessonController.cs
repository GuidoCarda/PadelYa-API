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
    // [Authorize]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        /// <summary>
        /// Crear una nueva clase (único o recurrente)
        /// </summary>
        [HttpPost]
        // [RequirePermission("class:create")]
        public async Task<IActionResult> CreateLesson([FromBody] LessonCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos", 
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            if (createDto.IsRecurrent)
            {
                var recurrentResult = await _lessonService.CreateRecurrentLessonsAsync(createDto);
                if (recurrentResult.Success)
                {
                    return Ok(recurrentResult);
                }
                return BadRequest(recurrentResult);
            }
            else
            {
                var singleResult = await _lessonService.CreateLessonAsync(createDto);
                if (singleResult.Success)
                {
                    return Ok(singleResult);
                }
                return BadRequest(singleResult);
            }
        }

        /// <summary>
        /// Obtener clase por ID
        /// </summary>
        [HttpGet("{id}")]
        // [RequirePermission("class:view")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var result = await _lessonService.GetLessonByIdAsync(id);
            
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
        /// Obtener lista de clases con filtros
        /// </summary>
        [HttpGet]
        // [RequirePermission("class:view")]
        public async Task<IActionResult> GetLessons([FromQuery] LessonFilterDto filterDto)
        {
            var result = await _lessonService.GetLessonsAsync(filterDto);
            
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Actualizar una clase existente
        /// </summary>
        [HttpPut("{id}")]
        // [RequirePermission("class:edit")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] LessonUpdateDto updateDto)
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

            updateDto.Id = id;
            var result = await _lessonService.UpdateLessonAsync(updateDto);
            
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
        /// Eliminar una clase
        /// </summary>
        [HttpDelete("{id}")]
        // [RequirePermission("class:cancel")] // Usamos cancel permission para delete también
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var result = await _lessonService.DeleteLessonAsync(id);
            
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
        /// Cancelar una clase
        /// </summary>
        [HttpPost("{id}/cancel")]
        // [RequirePermission("class:cancel")]
        public async Task<IActionResult> CancelLesson(int id)
        {
            var result = await _lessonService.CancelLessonAsync(id);
            
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
        /// Obtener clases por profesor
        /// </summary>
        [HttpGet("teacher/{teacherId}")]
        // [RequirePermission("class:view")]
        public async Task<IActionResult> GetLessonsByTeacher(
            int teacherId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _lessonService.GetLessonsByTeacherAsync(teacherId, startDate, endDate);
            
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener clases por cancha
        /// </summary>
        [HttpGet("court/{courtId}")]
        // [RequirePermission("class:view")]
        public async Task<IActionResult> GetLessonsByCourt(
            int courtId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _lessonService.GetLessonsByCourtAsync(courtId, startDate, endDate);
            
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Verificar disponibilidad de cancha
        /// </summary>
        [HttpPost("availability/check")]
        // [RequirePermission("class:view")]
        public async Task<IActionResult> CheckCourtAvailability([FromBody] CourtAvailabilityCheckDto checkDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var result = await _lessonService.ValidateCourtAvailabilityAsync(
                checkDto.CourtId, checkDto.Date, checkDto.StartTime, checkDto.EndTime, checkDto.ExcludeLessonId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener clases disponibles para inscripción
        /// </summary>
        [HttpGet("available")]
        // [RequirePermission("class:view")]
        public async Task<IActionResult> GetAvailableLessons([FromQuery] LessonFilterDto filterDto)
        {
            filterDto.AvailableOnly = true;
            var result = await _lessonService.GetLessonsAsync(filterDto);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener profesores para filtros
        /// </summary>
        [HttpGet("teachers")]
        // [RequirePermission("class:view")]
        public async Task<IActionResult> GetTeachers()
        {
            var result = await _lessonService.GetTeachersAsync();
            
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result);
        }
    }

    /// <summary>
    /// DTO para verificar disponibilidad de cancha
    /// </summary>
    public class CourtAvailabilityCheckDto
    {
        public int CourtId { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int? ExcludeLessonId { get; set; }
    }
} 