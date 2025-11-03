using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models.Lesson;
using padelya_api.Shared;
using padelya_api.models;

namespace padelya_api.Services
{
    public class LessonAttendanceService : ILessonAttendanceService
    {
        private readonly PadelYaDbContext _context;

        public LessonAttendanceService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage<LessonAttendanceDto>> RecordAttendanceAsync(LessonAttendanceCreateDto createDto, int teacherId)
        {
            try
            {
                // Validar que la clase existe y pertenece al profesor
                var lesson = await _context.Lessons
                    .Include(l => l.CourtSlot)
                    .Include(l => l.Enrollments)
                    .FirstOrDefaultAsync(l => l.Id == createDto.LessonId);

                if (lesson == null)
                {
                    return ResponseMessage<LessonAttendanceDto>.NotFound("Clase no encontrada");
                }

                if (lesson.TeacherId != teacherId)
                {
                    return ResponseMessage<LessonAttendanceDto>.Error("No tiene permisos para registrar asistencia en esta clase");
                }

                // Validar que el estudiante está inscrito
                var enrollment = await _context.LessonEnrollments
                    .FirstOrDefaultAsync(e => e.LessonId == createDto.LessonId && e.PersonId == createDto.PersonId);

                if (enrollment == null)
                {
                    return ResponseMessage<LessonAttendanceDto>.Error("El estudiante no está inscrito en esta clase");
                }

                // Verificar si ya existe un registro de asistencia
                var existing = await _context.LessonAttendances
                    .FirstOrDefaultAsync(a => a.LessonId == createDto.LessonId && a.PersonId == createDto.PersonId);

                if (existing != null)
                {
                    // Actualizar existente
                    existing.Status = createDto.Status;
                    existing.Notes = createDto.Notes;
                    existing.RecordedAt = DateTime.UtcNow;
                    existing.RecordedByTeacherId = teacherId;

                    _context.LessonAttendances.Update(existing);
                    await _context.SaveChangesAsync();

                    var dto = await MapToAttendanceDto(existing);
                    return ResponseMessage<LessonAttendanceDto>.SuccessResult(dto, "Asistencia actualizada exitosamente");
                }

                // Crear nuevo registro
                var attendance = new LessonAttendance
                {
                    LessonId = createDto.LessonId,
                    PersonId = createDto.PersonId,
                    Status = createDto.Status,
                    Notes = createDto.Notes,
                    RecordedAt = DateTime.UtcNow,
                    RecordedByTeacherId = teacherId
                };

                _context.LessonAttendances.Add(attendance);
                await _context.SaveChangesAsync();

                var responseDto = await MapToAttendanceDto(attendance);
                return ResponseMessage<LessonAttendanceDto>.SuccessResult(responseDto, "Asistencia registrada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<LessonAttendanceDto>.Error($"Error al registrar asistencia: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<LessonAttendanceDto>>> RecordBulkAttendanceAsync(LessonAttendanceBulkDto bulkDto, int teacherId)
        {
            try
            {
                // Validar que la clase existe y pertenece al profesor
                var lesson = await _context.Lessons
                    .Include(l => l.Enrollments)
                    .FirstOrDefaultAsync(l => l.Id == bulkDto.LessonId);

                if (lesson == null)
                {
                    return ResponseMessage<List<LessonAttendanceDto>>.NotFound("Clase no encontrada");
                }

                if (lesson.TeacherId != teacherId)
                {
                    return ResponseMessage<List<LessonAttendanceDto>>.Error("No tiene permisos para registrar asistencia en esta clase");
                }

                var attendances = new List<LessonAttendanceDto>();

                foreach (var attendanceDto in bulkDto.Attendances)
                {
                    attendanceDto.LessonId = bulkDto.LessonId;
                    var result = await RecordAttendanceAsync(attendanceDto, teacherId);
                    if (result.Success && result.Data != null)
                    {
                        attendances.Add(result.Data);
                    }
                }

                return ResponseMessage<List<LessonAttendanceDto>>.SuccessResult(attendances, 
                    $"Se registró asistencia para {attendances.Count} estudiantes");
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<LessonAttendanceDto>>.Error($"Error al registrar asistencia masiva: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<LessonAttendanceDto>>> GetAttendanceByLessonAsync(int lessonId)
        {
            try
            {
                var attendances = await _context.LessonAttendances
                    .Include(a => a.Person)
                    .Where(a => a.LessonId == lessonId)
                    .ToListAsync();

                var dtos = new List<LessonAttendanceDto>();
                foreach (var attendance in attendances)
                {
                    dtos.Add(await MapToAttendanceDto(attendance));
                }

                return ResponseMessage<List<LessonAttendanceDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<LessonAttendanceDto>>.Error($"Error al obtener asistencia: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<LessonAttendanceDto>>> GetAttendanceByStudentAsync(int personId)
        {
            try
            {
                var attendances = await _context.LessonAttendances
                    .Include(a => a.Lesson)
                    .ThenInclude(l => l.CourtSlot)
                    .Where(a => a.PersonId == personId)
                    .OrderByDescending(a => a.Lesson.CourtSlot.Date)
                    .ToListAsync();

                var dtos = new List<LessonAttendanceDto>();
                foreach (var attendance in attendances)
                {
                    dtos.Add(await MapToAttendanceDto(attendance));
                }

                return ResponseMessage<List<LessonAttendanceDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<LessonAttendanceDto>>.Error($"Error al obtener asistencia: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<LessonAttendanceDto>> UpdateAttendanceAsync(int attendanceId, AttendanceStatus status, string? notes, int teacherId)
        {
            try
            {
                var attendance = await _context.LessonAttendances
                    .Include(a => a.Lesson)
                    .FirstOrDefaultAsync(a => a.Id == attendanceId);

                if (attendance == null)
                {
                    return ResponseMessage<LessonAttendanceDto>.NotFound("Registro de asistencia no encontrado");
                }

                if (attendance.Lesson.TeacherId != teacherId)
                {
                    return ResponseMessage<LessonAttendanceDto>.Error("No tiene permisos para modificar esta asistencia");
                }

                attendance.Status = status;
                attendance.Notes = notes;
                attendance.RecordedAt = DateTime.UtcNow;
                attendance.RecordedByTeacherId = teacherId;

                _context.LessonAttendances.Update(attendance);
                await _context.SaveChangesAsync();

                var dto = await MapToAttendanceDto(attendance);
                return ResponseMessage<LessonAttendanceDto>.SuccessResult(dto, "Asistencia actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<LessonAttendanceDto>.Error($"Error al actualizar asistencia: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<AttendanceStatisticsDto>> GetAttendanceStatisticsByStudentAsync(int personId)
        {
            try
            {
                // Obtener todas las inscripciones del estudiante
                var enrollments = await _context.LessonEnrollments
                    .Include(e => e.Lesson)
                    .ThenInclude(l => l.CourtSlot)
                    .Where(e => e.PersonId == personId)
                    .ToListAsync();

                // Obtener todas las asistencias registradas del estudiante
                var attendances = await _context.LessonAttendances
                    .Where(a => a.PersonId == personId)
                    .ToListAsync();

                var totalClasses = enrollments.Count;
                var totalRegistered = attendances.Count;
                var presentCount = attendances.Count(a => a.Status == AttendanceStatus.Present);
                var absentCount = attendances.Count(a => a.Status == AttendanceStatus.Absent);
                var justifiedCount = attendances.Count(a => a.Status == AttendanceStatus.Justified);

                // Calcular porcentaje de asistencia (solo contar presentes y ausentes, no justificados)
                var attendancePercentage = totalRegistered > 0
                    ? (double)presentCount / (presentCount + absentCount) * 100
                    : 0;

                var statistics = new AttendanceStatisticsDto
                {
                    TotalClasses = totalClasses,
                    TotalRegistered = totalRegistered,
                    PresentCount = presentCount,
                    AbsentCount = absentCount,
                    JustifiedCount = justifiedCount,
                    AttendancePercentage = Math.Round(attendancePercentage, 2)
                };

                return ResponseMessage<AttendanceStatisticsDto>.SuccessResult(statistics);
            }
            catch (Exception ex)
            {
                return ResponseMessage<AttendanceStatisticsDto>.Error($"Error al obtener estadísticas de asistencia: {ex.Message}");
            }
        }

        private async Task<LessonAttendanceDto> MapToAttendanceDto(LessonAttendance attendance)
        {
            await _context.Entry(attendance)
                .Reference(a => a.Person)
                .LoadAsync();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PersonId == attendance.PersonId);

            return new LessonAttendanceDto
            {
                Id = attendance.Id,
                LessonId = attendance.LessonId,
                PersonId = attendance.PersonId,
                StudentName = user?.Name ?? "N/A",
                StudentSurname = user?.Surname ?? "N/A",
                Status = attendance.Status,
                Notes = attendance.Notes,
                RecordedAt = attendance.RecordedAt,
                RecordedByTeacherId = attendance.RecordedByTeacherId
            };
        }
    }
}

