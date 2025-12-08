using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using padelya_api.Attributes;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models.Lesson;
using padelya_api.Services;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonAttendanceController : ControllerBase
    {
        private readonly ILessonAttendanceService _attendanceService;
        private readonly PadelYaDbContext _context;

        public LessonAttendanceController(ILessonAttendanceService attendanceService, PadelYaDbContext context)
        {
            _attendanceService = attendanceService;
            _context = context;
        }

        /// <summary>
        /// Registrar asistencia de un estudiante
        /// </summary>
        [HttpPost]
        [RequirePermission("lesson:assign_user")] // Solo profesores pueden registrar asistencia
        public async Task<IActionResult> RecordAttendance([FromBody] LessonAttendanceCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            // Obtener el PersonId del usuario (que es el TeacherId en la tabla Lessons)
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PersonId == null)
            {
                return BadRequest(ResponseMessage<object>.Error("Usuario no encontrado o sin persona asociada"));
            }
            
            var result = await _attendanceService.RecordAttendanceAsync(createDto, user.PersonId.Value);
            
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
        /// Registrar asistencia masiva para una clase
        /// </summary>
        [HttpPost("bulk")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> RecordBulkAttendance([FromBody] LessonAttendanceBulkDto bulkDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            // Obtener el PersonId del usuario (que es el TeacherId en la tabla Lessons)
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PersonId == null)
            {
                return BadRequest(ResponseMessage<object>.Error("Usuario no encontrado o sin persona asociada"));
            }
            
            var result = await _attendanceService.RecordBulkAttendanceAsync(bulkDto, user.PersonId.Value);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener asistencia de una clase
        /// </summary>
        [HttpGet("lesson/{lessonId}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetAttendanceByLesson(int lessonId)
        {
            var result = await _attendanceService.GetAttendanceByLessonAsync(lessonId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener asistencia de un estudiante
        /// </summary>
        [HttpGet("student/{personId}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetAttendanceByStudent(int personId)
        {
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var userRoleId = int.Parse(User.FindFirst("role_id")?.Value ?? "0");
            
            // Verificar que el usuario solo vea sus propias asistencias (a menos que sea admin)
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.PersonId != personId && userRoleId != 100) // 100 = Admin
            {
                return Forbid();
            }

            var result = await _attendanceService.GetAttendanceByStudentAsync(personId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener estadísticas de asistencia de un estudiante
        /// </summary>
        [HttpGet("student/{personId}/statistics")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetAttendanceStatisticsByStudent(int personId)
        {
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var userRoleId = int.Parse(User.FindFirst("role_id")?.Value ?? "0");
            
            // Verificar que el usuario solo vea sus propias estadísticas (a menos que sea admin)
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.PersonId != personId && userRoleId != 100) // 100 = Admin
            {
                return Forbid();
            }

            var result = await _attendanceService.GetAttendanceStatisticsByStudentAsync(personId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Actualizar registro de asistencia
        /// </summary>
        [HttpPut("{attendanceId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> UpdateAttendance(int attendanceId, [FromBody] UpdateAttendanceDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            // Obtener el PersonId del usuario (que es el TeacherId en la tabla Lessons)
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PersonId == null)
            {
                return BadRequest(ResponseMessage<object>.Error("Usuario no encontrado o sin persona asociada"));
            }
            
            var result = await _attendanceService.UpdateAttendanceAsync(
                attendanceId, 
                updateDto.Status, 
                updateDto.Notes, 
                user.PersonId.Value);
            
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

    public class UpdateAttendanceDto
    {
        public AttendanceStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}

