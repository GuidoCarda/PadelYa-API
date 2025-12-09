using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models;
using padelya_api.Shared;
using padelya_api.Constants;
using padelya_api.models;
using System.Security.Claims;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;

namespace padelya_api.Services
{
    public class LessonEnrollmentService : ILessonEnrollmentService
    {
        private readonly PadelYaDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public LessonEnrollmentService(
            PadelYaDbContext context, 
            IPaymentService paymentService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _context = context;
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
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

                // Limpiar inscripciones expiradas antes de validar cupos
                await CleanupExpiredEnrollmentsAsync(createDto.LessonId);

                // Validar cupos disponibles contando solo inscripciones confirmadas
                // (con pago aprobado o sin requerimiento de pago)
                var confirmedEnrollmentsCount = await _context.LessonEnrollments
                    .Include(e => e.Payment)
                    .Where(e => e.LessonId == createDto.LessonId && 
                                (e.Payment == null || e.Payment.PaymentStatus == PaymentStatus.Approved))
                    .CountAsync();

                if (confirmedEnrollmentsCount >= lesson.MaxCapacity)
                {
                    return ResponseMessage<LessonEnrollmentResponseDto>.Error("La clase está llena, no hay cupos disponibles");
                }

                // Validar que la persona exista
                var person = await _context.Set<Person>().FindAsync(createDto.PersonId);
                if (person == null)
                {
                    return ResponseMessage<LessonEnrollmentResponseDto>.NotFound("Persona no encontrada");
                }

                // Verificar si ya está inscrito con pago aprobado o sin pago
                var existingEnrollment = await _context.LessonEnrollments
                    .Include(e => e.Payment)
                    .FirstOrDefaultAsync(e => e.LessonId == createDto.LessonId && 
                                             e.PersonId == createDto.PersonId &&
                                             (e.Payment == null || e.Payment.PaymentStatus == PaymentStatus.Approved));

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

                // Si requiere pago, crear el pago pendiente (se usará MercadoPago)
                // Si NO requiere pago, se asume pago manual/efectivo y se crea pago aprobado
                if (lesson.Price > 0)
                {
                    var payment = new Payment
                    {
                        Amount = lesson.Price,
                        PaymentMethod = createDto.RequiresPayment ? "pending" : "Efectivo",
                        PaymentStatus = createDto.RequiresPayment ? PaymentStatus.Pending : PaymentStatus.Approved,
                        PaymentType = PaymentType.Total,
                        LessonEnrollmentId = enrollment.Id,
                        PersonId = createDto.PersonId,
                        CreatedAt = DateTime.UtcNow,
                        TransactionId = createDto.RequiresPayment 
                            ? $"LESSON-{enrollment.Id}-{DateTime.UtcNow.Ticks}"
                            : $"MANUAL-LESSON-{enrollment.Id}-{DateTime.UtcNow.Ticks}"
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

        public async Task<LessonEnrollmentInitPointDto> EnrollWithPaymentAsync(int lessonId)
        {
            // Obtener el ID del usuario autenticado
            var loggedInUserIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue("user_id");
            if (string.IsNullOrEmpty(loggedInUserIdString))
            {
                throw new ArgumentException("No se pudo identificar al usuario. Inicie sesión nuevamente.");
            }
            var loggedInUserId = int.Parse(loggedInUserIdString);

            // Obtener el personId del usuario
            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == loggedInUserId);

            if (user == null || user.PersonId == null)
            {
                throw new ArgumentException("Usuario no tiene una persona asociada");
            }

            var personId = user.PersonId.Value;

            // Validar que la clase exista y esté disponible
            var lesson = await _context.Lessons
                .Include(l => l.CourtSlot)
                .Include(l => l.Enrollments)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
            {
                throw new ArgumentException("Clase no encontrada");
            }

            if (lesson.HasStarted)
            {
                throw new ArgumentException("No se puede inscribir a una clase que ya comenzó");
            }

            // Limpiar inscripciones expiradas sin pago antes de validar cupos
            await CleanupExpiredEnrollmentsAsync(lessonId);

            // Cancelar inscripciones pendientes del usuario actual (permite reintentar inmediatamente)
            await CancelUserPendingEnrollmentsAsync(lessonId, personId);

            // Verificar cupos disponibles (solo contar inscripciones con pago aprobado)
            var confirmedEnrollmentsCount = await _context.LessonEnrollments
                .Include(e => e.Payment)
                .Where(e => e.LessonId == lessonId && 
                            e.Payment != null && 
                            e.Payment.PaymentStatus == PaymentStatus.Approved)
                .CountAsync();

            if (confirmedEnrollmentsCount >= lesson.MaxCapacity)
            {
                throw new ArgumentException("La clase está llena, no hay cupos disponibles");
            }

            // Verificar si ya está inscrito con pago aprobado
            var existingEnrollment = await _context.LessonEnrollments
                .Include(e => e.Payment)
                .FirstOrDefaultAsync(e => e.LessonId == lessonId && 
                                         e.PersonId == personId &&
                                         e.Payment != null &&
                                         e.Payment.PaymentStatus == PaymentStatus.Approved);

            if (existingEnrollment != null)
            {
                throw new ArgumentException("Ya estás inscrito en esta clase");
            }

            // Crear inscripción con estado pendiente de pago
            var enrollment = new LessonEnrollment
            {
                LessonId = lessonId,
                PersonId = personId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.LessonEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Crear preferencia de MercadoPago
            var preference = await CreateMercadoPagoPreference(enrollment, lesson.Price, personId);

            if (preference == null)
                throw new Exception("Preference not created");

            var response = new LessonEnrollmentInitPointDto
            {
                init_point = preference.InitPoint
            };

            return response;
        }

        private async Task CleanupExpiredEnrollmentsAsync(int lessonId)
        {
            // Eliminar inscripciones sin pago aprobado que fueron creadas hace más de 15 minutos
            // También eliminar inscripciones con pago rechazado
            var expiredEnrollments = await _context.LessonEnrollments
                .Include(e => e.Payment)
                .Where(e => e.LessonId == lessonId &&
                           (e.Payment == null || 
                            e.Payment.PaymentStatus == PaymentStatus.Rejected ||
                            (e.Payment.PaymentStatus != PaymentStatus.Approved && 
                             e.EnrollmentDate <= DateTime.UtcNow.AddMinutes(-15))))
                .ToListAsync();

            if (expiredEnrollments.Any())
            {
                foreach (var enrollment in expiredEnrollments)
                {
                    // Si tiene un pago pendiente o rechazado, eliminarlo también
                    if (enrollment.Payment != null && 
                        enrollment.Payment.PaymentStatus != PaymentStatus.Approved)
                    {
                        _context.Payments.Remove(enrollment.Payment);
                    }
                    _context.LessonEnrollments.Remove(enrollment);
                }
                await _context.SaveChangesAsync();
                Console.WriteLine($"[Lesson Payment] Cleaned up {expiredEnrollments.Count} expired/rejected enrollments for lesson {lessonId}");
            }
        }

        private async Task CancelUserPendingEnrollmentsAsync(int lessonId, int personId)
        {
            // Cancelar inscripciones pendientes del usuario actual
            var pendingEnrollments = await _context.LessonEnrollments
                .Include(e => e.Payment)
                .Where(e => e.LessonId == lessonId &&
                           e.PersonId == personId &&
                           (e.Payment == null || 
                            e.Payment.PaymentStatus != PaymentStatus.Approved))
                .ToListAsync();

            if (pendingEnrollments.Any())
            {
                foreach (var enrollment in pendingEnrollments)
                {
                    if (enrollment.Payment != null)
                    {
                        _context.Payments.Remove(enrollment.Payment);
                    }
                    _context.LessonEnrollments.Remove(enrollment);
                }
                await _context.SaveChangesAsync();
                Console.WriteLine($"[Lesson Payment] Cancelled {pendingEnrollments.Count} pending enrollments for user {personId} in lesson {lessonId}");
            }
        }

        private async Task<Preference> CreateMercadoPagoPreference(LessonEnrollment enrollment, decimal amount, int personId)
        {
            try
            {
                MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];

                Console.WriteLine($"[Lesson Payment] Creating MercadoPago preference for enrollment {enrollment.Id}");
                Console.WriteLine($"[Lesson Payment] Amount: {amount}, PersonId: {personId}");

                var externalReference = $"lesson_{enrollment.LessonId}_enrollment_{enrollment.Id}";

                var request = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                    {
                        new PreferenceItemRequest
                        {
                            Title = $"Inscripción a clase",
                            Quantity = 1,
                            CurrencyId = "ARS",
                            UnitPrice = amount
                        }
                    },
                    ExternalReference = externalReference,
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = "https://x8dgxb6z-3000.brs.devtunnels.ms/lessons/payment/success",
                        Failure = "https://x8dgxb6z-3000.brs.devtunnels.ms/lessons/payment/failure",
                        Pending = "https://x8dgxb6z-3000.brs.devtunnels.ms/lessons/payment/pending"
                    },
                    NotificationUrl = "https://x8dgxb6z-5105.brs.devtunnels.ms/api/Payments/webhook",
                    Metadata = new Dictionary<string, object>
                    {
                        ["lesson_enrollment_id"] = enrollment.Id.ToString(),
                        ["person_id"] = personId.ToString(),
                        ["lesson_id"] = enrollment.LessonId.ToString()
                    },
                    AutoReturn = "approved",
                    Expires = true,
                    ExpirationDateTo = DateTime.UtcNow.AddMinutes(15),
                    DateOfExpiration = DateTime.UtcNow.AddMinutes(15),
                    PaymentMethods = new()
                    {
                        Installments = 1,
                        ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                        {
                            new PreferencePaymentTypeRequest
                            {
                                Id = "ticket",
                            }
                        }
                    }
                };

                var client = new PreferenceClient();
                var preference = await client.CreateAsync(request);

                Console.WriteLine($"[Lesson Payment] Preference created successfully: {preference.Id}");
                Console.WriteLine($"[Lesson Payment] Init Point: {preference.InitPoint}");

                return preference;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lesson Payment ERROR] Failed to create MercadoPago preference: {ex.Message}");
                Console.WriteLine($"[Lesson Payment ERROR] Stack trace: {ex.StackTrace}");
                throw;
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
                // Limpiar inscripciones expiradas primero
                await CleanupExpiredEnrollmentsAsync(lessonId);

                // Obtener todas las inscripciones (incluyendo pendientes de pago)
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
                    var isPaid = e.Payment != null && e.Payment.PaymentStatus == PaymentStatus.Approved;
                    
                    // Mapear el estado de pago a español
                    string paymentStatus;
                    if (e.Payment == null)
                    {
                        paymentStatus = "Pendiente";
                    }
                    else
                    {
                        paymentStatus = e.Payment.PaymentStatus switch
                        {
                            PaymentStatus.Approved => "Pagado",
                            PaymentStatus.Pending => "Pendiente",
                            PaymentStatus.Rejected => "Rechazado",
                            _ => "Pendiente"
                        };
                    }
                    
                    return new LessonEnrollmentListDto
                    {
                        Id = e.Id,
                        PersonId = e.PersonId,
                        StudentName = user?.Name ?? "N/A",
                        StudentSurname = user?.Surname ?? "N/A",
                        StudentEmail = user?.Email ?? "N/A",
                        StudentCategory = e.Person?.Category ?? "",
                        EnrollmentDate = e.EnrollmentDate,
                        IsPaid = isPaid,
                        PaymentStatus = paymentStatus
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
                // Obtener todas las inscripciones (incluyendo pendientes de pago)
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

