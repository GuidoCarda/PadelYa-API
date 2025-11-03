using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models;
using padelya_api.Shared;
using padelya_api.Constants;
using padelya_api.models;

namespace padelya_api.Services
{
    public class LessonEnrollmentService : ILessonEnrollmentService
    {
        private readonly PadelYaDbContext _context;
        private readonly IPaymentService _paymentService;

        public LessonEnrollmentService(PadelYaDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<ResponseMessage<LessonEnrollmentResponseDto>> EnrollStudentAsync(LessonEnrollmentCreateDto createDto, int? userId = null)
        {
            try
            {
                // Validar que la clase exista y esté disponible
                var lesson = await _context.Lessons
                    .Include(l => l.CourtSlot)
                    .Include(l => l.Enrollments)
                    .FirstOrDefaultAsync(l => l.Id == createDto.LessonId);

                if (lesson == null)
                {
                    return ResponseMessage<LessonEnrollmentResponseDto>.NotFound("Clase no encontrada");
                }

                if (lesson.HasStarted)
                {
                    return ResponseMessage<LessonEnrollmentResponseDto>.Error("No se puede inscribir a una clase que ya comenzó");
                }

                if (!lesson.IsAvailable)
                {
                    return ResponseMessage<LessonEnrollmentResponseDto>.Error("La clase está llena, no hay cupos disponibles");
                }

                // Validar que la persona exista
                var person = await _context.Set<Person>().FindAsync(createDto.PersonId);
                if (person == null)
                {
                    return ResponseMessage<LessonEnrollmentResponseDto>.NotFound("Persona no encontrada");
                }

                // Verificar si ya está inscrito
                var existingEnrollment = await _context.LessonEnrollments
                    .FirstOrDefaultAsync(e => e.LessonId == createDto.LessonId && e.PersonId == createDto.PersonId);

                if (existingEnrollment != null)
                {
                    return ResponseMessage<LessonEnrollmentResponseDto>.Error("El estudiante ya está inscrito en esta clase");
                }

                // Crear inscripción
                var enrollment = new LessonEnrollment
                {
                    LessonId = createDto.LessonId,
                    PersonId = createDto.PersonId,
                    EnrollmentDate = DateTime.UtcNow
                };

                _context.LessonEnrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                // Si requiere pago, crear el pago (esto puede ser expandido con MercadoPago)
                if (createDto.RequiresPayment && lesson.Price > 0)
                {
                    var payment = new Payment
                    {
                        Amount = lesson.Price,
                        PaymentMethod = "pending", // Se actualizará cuando se procese el pago
                        PaymentStatus = PaymentStatus.Pending,
                        PaymentType = PaymentType.Total,
                        LessonEnrollmentId = enrollment.Id,
                        PersonId = createDto.PersonId,
                        CreatedAt = DateTime.UtcNow,
                        TransactionId = $"LESSON-{enrollment.Id}-{DateTime.UtcNow.Ticks}"
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                }

                // Cargar relaciones para la respuesta
                await _context.Entry(enrollment)
                    .Reference(e => e.Lesson)
                    .LoadAsync();

                await _context.Entry(enrollment.Lesson)
                    .Reference(l => l.CourtSlot)
                    .LoadAsync();

                await _context.Entry(enrollment)
                    .Reference(e => e.Person)
                    .LoadAsync();

                // Obtener información del usuario si existe
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.PersonId == createDto.PersonId);

                var responseDto = MapToEnrollmentResponseDto(enrollment, user);
                return ResponseMessage<LessonEnrollmentResponseDto>.SuccessResult(responseDto, "Inscripción realizada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<LessonEnrollmentResponseDto>.Error($"Error al inscribir al estudiante: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<bool>> CancelEnrollmentAsync(int enrollmentId, int? userId = null)
        {
            try
            {
                var enrollment = await _context.LessonEnrollments
                    .Include(e => e.Lesson)
                    .ThenInclude(l => l.CourtSlot)
                    .Include(e => e.Payment)
                    .FirstOrDefaultAsync(e => e.Id == enrollmentId);

                if (enrollment == null)
                {
                    return ResponseMessage<bool>.NotFound("Inscripción no encontrada");
                }

                // Validar que el usuario tenga permiso (solo el mismo estudiante o admin)
                if (userId.HasValue)
                {
                    var user = await _context.Users.FindAsync(userId.Value);
                    if (user != null && user.PersonId != enrollment.PersonId && user.RoleId != 100) // 100 = Admin
                    {
                        return ResponseMessage<bool>.Error("No tiene permisos para cancelar esta inscripción");
                    }
                }

                if (enrollment.Lesson.HasStarted)
                {
                    return ResponseMessage<bool>.Error("No se puede cancelar la inscripción de una clase que ya comenzó");
                }

                // Aquí se podría aplicar políticas de cancelación (reembolso, crédito, etc.)
                // Por ahora simplemente eliminamos la inscripción

                // Si hay un pago pendiente, se puede manejar el reembolso aquí
                if (enrollment.Payment != null && enrollment.Payment.PaymentStatus == PaymentStatus.Approved)
                {
                    // Lógica de reembolso según políticas (esto puede ser expandido)
                }

                _context.LessonEnrollments.Remove(enrollment);
                await _context.SaveChangesAsync();

                return ResponseMessage<bool>.SuccessResult(true, "Inscripción cancelada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<bool>.Error($"Error al cancelar la inscripción: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<LessonEnrollmentListDto>>> GetEnrollmentsByLessonAsync(int lessonId)
        {
            try
            {
                var enrollments = await _context.LessonEnrollments
                    .Include(e => e.Person)
                    .Include(e => e.Payment)
                    .Where(e => e.LessonId == lessonId)
                    .ToListAsync();

                var users = await _context.Users
                    .Where(u => enrollments.Select(e => e.PersonId).Contains(u.PersonId ?? -1))
                    .ToListAsync();

                var dtos = enrollments.Select(e =>
                {
                    var user = users.FirstOrDefault(u => u.PersonId == e.PersonId);
                    return new LessonEnrollmentListDto
                    {
                        Id = e.Id,
                        PersonId = e.PersonId,
                        StudentName = user?.Name ?? "N/A",
                        StudentSurname = user?.Surname ?? "N/A",
                        StudentEmail = user?.Email ?? "N/A",
                        StudentCategory = e.Person?.Category ?? "",
                        EnrollmentDate = e.EnrollmentDate,
                        IsPaid = e.Payment != null && e.Payment.PaymentStatus == PaymentStatus.Approved
                    };
                }).ToList();

                return ResponseMessage<List<LessonEnrollmentListDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<LessonEnrollmentListDto>>.Error($"Error al obtener las inscripciones: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<LessonEnrollmentResponseDto>>> GetEnrollmentsByStudentAsync(int personId)
        {
            try
            {
                var enrollments = await _context.LessonEnrollments
                    .Include(e => e.Lesson)
                    .ThenInclude(l => l.CourtSlot)
                    .Include(e => e.Person)
                    .Include(e => e.Payment)
                    .Where(e => e.PersonId == personId)
                    .OrderByDescending(e => e.Lesson.CourtSlot.Date)
                    .ToListAsync();

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.PersonId == personId);

                var dtos = enrollments.Select(e => MapToEnrollmentResponseDto(e, user)).ToList();

                return ResponseMessage<List<LessonEnrollmentResponseDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<LessonEnrollmentResponseDto>>.Error($"Error al obtener las inscripciones: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<bool>> AdminEnrollStudentAsync(int lessonId, int personId)
        {
            var createDto = new LessonEnrollmentCreateDto
            {
                LessonId = lessonId,
                PersonId = personId,
                RequiresPayment = false // Admin puede inscribir sin pago (ej: pago en efectivo)
            };

            var result = await EnrollStudentAsync(createDto);
            return result.Success 
                ? ResponseMessage<bool>.SuccessResult(true, result.Message ?? "Estudiante inscrito exitosamente")
                : ResponseMessage<bool>.Error(result.Message ?? "Error al inscribir al estudiante");
        }

        public async Task<ResponseMessage<bool>> AdminRemoveEnrollmentAsync(int enrollmentId)
        {
            return await CancelEnrollmentAsync(enrollmentId, null);
        }

        public async Task<ResponseMessage<bool>> UpdateEnrollmentPaymentStatusAsync(int enrollmentId, string paymentStatus)
        {
            try
            {
                var enrollment = await _context.LessonEnrollments
                    .Include(e => e.Payment)
                    .FirstOrDefaultAsync(e => e.Id == enrollmentId);

                if (enrollment == null)
                {
                    return ResponseMessage<bool>.NotFound("Inscripción no encontrada");
                }

                // Determinar el nuevo estado de pago
                PaymentStatus newPaymentStatus = paymentStatus.ToLower() switch
                {
                    "pagado" or "approved" => PaymentStatus.Approved,
                    "pendiente" or "pending" => PaymentStatus.Pending,
                    "rechazado" or "rejected" => PaymentStatus.Rejected,
                    _ => PaymentStatus.Pending
                };

                if (enrollment.Payment == null)
                {
                    // Si no tiene pago, crear uno nuevo
                    var lesson = await _context.Lessons.FindAsync(enrollment.LessonId);
                    if (lesson == null)
                    {
                        return ResponseMessage<bool>.Error("Clase no encontrada");
                    }

                    var newPayment = new Payment
                    {
                        Amount = lesson.Price,
                        PaymentMethod = "Manual",
                        PaymentStatus = newPaymentStatus,
                        PaymentType = PaymentType.Total,
                        LessonEnrollmentId = enrollment.Id,
                        PersonId = enrollment.PersonId,
                        CreatedAt = DateTime.UtcNow,
                        TransactionId = $"manual_{enrollmentId}_{DateTime.UtcNow.Ticks}"
                    };

                    _context.Payments.Add(newPayment);
                    // No necesitamos actualizar el enrollment aquí, solo guardar el payment
                }
                else
                {
                    // Actualizar estado del pago existente
                    enrollment.Payment.PaymentStatus = newPaymentStatus;
                }

                await _context.SaveChangesAsync();

                return ResponseMessage<bool>.SuccessResult(true, "Estado de pago actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<bool>.Error($"Error al actualizar el estado de pago: {ex.Message}");
            }
        }

        private LessonEnrollmentResponseDto MapToEnrollmentResponseDto(LessonEnrollment enrollment, User? user)
        {
            return new LessonEnrollmentResponseDto
            {
                Id = enrollment.Id,
                LessonId = enrollment.LessonId,
                LessonDescription = enrollment.Lesson?.Description ?? "",
                LessonDate = enrollment.Lesson?.CourtSlot?.Date ?? DateTime.MinValue,
                LessonStartTime = enrollment.Lesson?.CourtSlot?.StartTime ?? TimeOnly.MinValue,
                PersonId = enrollment.PersonId,
                StudentName = user?.Name ?? "N/A",
                StudentSurname = user?.Surname ?? "N/A",
                StudentEmail = user?.Email ?? "N/A",
                StudentCategory = enrollment.Person?.Category ?? "",
                EnrollmentDate = enrollment.EnrollmentDate,
                IsPaid = enrollment.Payment != null && enrollment.Payment.PaymentStatus == PaymentStatus.Approved,
                PaymentAmount = enrollment.Payment?.Amount,
                PaymentDate = enrollment.Payment?.CreatedAt
            };
        }
    }
}

