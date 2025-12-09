using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class LessonEnrollmentController : ControllerBase
    {
        private readonly ILessonEnrollmentService _enrollmentService;
        private readonly PadelYaDbContext _context;

        public LessonEnrollmentController(ILessonEnrollmentService enrollmentService, PadelYaDbContext context)
        {
            _enrollmentService = enrollmentService;
            _context = context;
        }

        /// <summary>
        /// Inscribir un estudiante a una clase
        /// </summary>
        [HttpPost]
        [RequirePermission("lesson:join")]
        public async Task<IActionResult> Enroll([FromBody] LessonEnrollmentCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessage<object>.ValidationError(
                    "Datos de entrada inválidos",
                    ModelState.ToDictionary(x => x.Key, x => x.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>())));
            }

            // Obtener userId del token (esto se puede mejorar con un helper)
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            
            var result = await _enrollmentService.EnrollStudentAsync(createDto, userId);
            
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
        /// Inscribir a una clase con pago online (MercadoPago)
        /// </summary>
        [HttpPost("{lessonId}/enroll-with-payment")]
        [RequirePermission("lesson:join")]
        public async Task<IActionResult> EnrollWithPayment(int lessonId)
        {
            try
            {
                var result = await _enrollmentService.EnrollWithPaymentAsync(lessonId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancelar inscripción a una clase
        /// </summary>
        [HttpDelete("{enrollmentId}")]
        [RequirePermission("lesson:leave")]
        public async Task<IActionResult> CancelEnrollment(int enrollmentId)
        {
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            
            var result = await _enrollmentService.CancelEnrollmentAsync(enrollmentId, userId);
            
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
        /// Cancelar inscripción por lessonId (para jugadores)
        /// </summary>
        [HttpDelete("lesson/{lessonId}")]
        [RequirePermission("lesson:leave")]
        public async Task<IActionResult> CancelEnrollmentByLesson(int lessonId)
        {
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null || !user.PersonId.HasValue)
            {
                return BadRequest("Usuario no encontrado o sin persona asociada");
            }

            // Buscar la inscripción del usuario en esta clase
            var enrollment = await _context.LessonEnrollments
                .FirstOrDefaultAsync(e => e.LessonId == lessonId && e.PersonId == user.PersonId.Value);

            if (enrollment == null)
            {
                return NotFound("No estás inscrito en esta clase");
            }

            var result = await _enrollmentService.CancelEnrollmentAsync(enrollment.Id, userId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener inscripciones de una clase (admin/profesor)
        /// </summary>
        [HttpGet("lesson/{lessonId}")]
        [RequirePermission("lesson:view")]
        public async Task<IActionResult> GetEnrollmentsByLesson(int lessonId)
        {
            var result = await _enrollmentService.GetEnrollmentsByLessonAsync(lessonId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Obtener inscripciones de un estudiante
        /// </summary>
        [HttpGet("student/{personId}")]
        [RequirePermission("lesson:view")] // Cambiado a lesson:view para que jugadores puedan ver sus inscripciones
        public async Task<IActionResult> GetEnrollmentsByStudent(int personId)
        {
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var userRoleId = int.Parse(User.FindFirst("role_id")?.Value ?? "0");
            
            // Verificar que el usuario solo vea sus propias inscripciones (a menos que sea admin)
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.PersonId != personId && userRoleId != 100) // 100 = Admin
            {
                return Forbid();
            }

            var result = await _enrollmentService.GetEnrollmentsByStudentAsync(personId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Inscribir estudiante manualmente (solo admin)
        /// </summary>
        [HttpPost("admin/{lessonId}/{personId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> AdminEnroll(int lessonId, int personId)
        {
            var result = await _enrollmentService.AdminEnrollStudentAsync(lessonId, personId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Remover inscripción manualmente (solo admin)
        /// </summary>
        [HttpDelete("admin/{enrollmentId}")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> AdminRemoveEnrollment(int enrollmentId)
        {
            var result = await _enrollmentService.AdminRemoveEnrollmentAsync(enrollmentId);
            
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
        /// Actualizar estado de pago de una inscripción (solo admin)
        /// </summary>
        [HttpPut("{enrollmentId}/payment-status")]
        [RequirePermission("lesson:assign_user")]
        public async Task<IActionResult> UpdatePaymentStatus(int enrollmentId, [FromBody] UpdatePaymentStatusDto statusDto)
        {
            var result = await _enrollmentService.UpdateEnrollmentPaymentStatusAsync(enrollmentId, statusDto.PaymentStatus);
            
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


