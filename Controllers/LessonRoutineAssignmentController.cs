using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.Attributes;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Services;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonRoutineAssignmentController : ControllerBase
    {
        private readonly ILessonRoutineAssignmentService _assignmentService;
        private readonly PadelYaDbContext _context;

        public LessonRoutineAssignmentController(
            ILessonRoutineAssignmentService assignmentService,
            PadelYaDbContext context)
        {
            _assignmentService = assignmentService;
            _context = context;
        }

        /// <summary>
        /// Obtener asignaciones de rutinas de una clase
        /// </summary>
        [HttpGet("lesson/{lessonId}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetAssignmentsByLesson(int lessonId)
        {
            var result = await _assignmentService.GetAssignmentsByLessonAsync(lessonId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Asignar rutinas masivamente a alumnos de una clase
        /// </summary>
        [HttpPost("bulk")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> AssignRoutinesBulk([FromBody] LessonRoutineAssignmentBulkDto bulkDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PersonId == null)
            {
                return BadRequest(ResponseMessage<object>.Error("Usuario no encontrado o sin persona asociada"));
            }
            
            var result = await _assignmentService.AssignRoutinesBulkAsync(bulkDto, user.PersonId.Value);
            
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
        /// Asignar rutina a un alumno en una clase
        /// </summary>
        [HttpPost("lesson/{lessonId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> AssignRoutine(int lessonId, [FromBody] LessonRoutineAssignmentCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PersonId == null)
            {
                return BadRequest(ResponseMessage<object>.Error("Usuario no encontrado o sin persona asociada"));
            }
            
            var result = await _assignmentService.AssignRoutineAsync(createDto, lessonId, user.PersonId.Value);
            
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
        /// Eliminar asignación de rutina
        /// </summary>
        [HttpDelete("{assignmentId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> RemoveAssignment(int assignmentId)
        {
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PersonId == null)
            {
                return BadRequest(ResponseMessage<object>.Error("Usuario no encontrado o sin persona asociada"));
            }
            
            var result = await _assignmentService.RemoveAssignmentAsync(assignmentId, user.PersonId.Value);
            
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

