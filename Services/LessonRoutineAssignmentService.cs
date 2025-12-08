using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public interface ILessonRoutineAssignmentService
    {
        Task<ResponseMessage<List<LessonRoutineAssignmentDto>>> GetAssignmentsByLessonAsync(int lessonId);
        Task<ResponseMessage<List<LessonRoutineAssignmentDto>>> AssignRoutinesBulkAsync(LessonRoutineAssignmentBulkDto bulkDto, int teacherId);
        Task<ResponseMessage<LessonRoutineAssignmentDto>> AssignRoutineAsync(LessonRoutineAssignmentCreateDto createDto, int lessonId, int teacherId);
        Task<ResponseMessage<object>> RemoveAssignmentAsync(int assignmentId, int teacherId);
    }

    public class LessonRoutineAssignmentService : ILessonRoutineAssignmentService
    {
        private readonly PadelYaDbContext _context;

        public LessonRoutineAssignmentService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage<List<LessonRoutineAssignmentDto>>> GetAssignmentsByLessonAsync(int lessonId)
        {
            try
            {
                var assignments = await _context.LessonRoutineAssignments
                    .Include(a => a.Person)
                    .Include(a => a.Routine)
                    .Where(a => a.LessonId == lessonId)
                    .ToListAsync();

                var dtos = assignments.Select(a => new LessonRoutineAssignmentDto
                {
                    Id = a.Id,
                    LessonId = a.LessonId,
                    PersonId = a.PersonId,
                    PersonName = a.Person.Name,
                    PersonSurname = a.Person.Surname,
                    RoutineId = a.RoutineId,
                    RoutineCategory = a.Routine.Category,
                    RoutineDescription = a.Routine.Description,
                    AssignedAt = a.AssignedAt
                }).ToList();

                return ResponseMessage<List<LessonRoutineAssignmentDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<LessonRoutineAssignmentDto>>.Error($"Error al obtener asignaciones: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<LessonRoutineAssignmentDto>>> AssignRoutinesBulkAsync(LessonRoutineAssignmentBulkDto bulkDto, int teacherId)
        {
            try
            {
                // Validar que la clase existe y pertenece al profesor
                var lesson = await _context.Lessons
                    .Include(l => l.Enrollments)
                    .FirstOrDefaultAsync(l => l.Id == bulkDto.LessonId);

                if (lesson == null)
                {
                    return ResponseMessage<List<LessonRoutineAssignmentDto>>.NotFound("Clase no encontrada");
                }

                if (lesson.TeacherId != teacherId)
                {
                    return ResponseMessage<List<LessonRoutineAssignmentDto>>.Error("No tiene permisos para asignar rutinas en esta clase");
                }

                var resultAssignments = new List<LessonRoutineAssignmentDto>();

                // Eliminar asignaciones existentes para esta clase
                var existingAssignments = await _context.LessonRoutineAssignments
                    .Where(a => a.LessonId == bulkDto.LessonId)
                    .ToListAsync();
                _context.LessonRoutineAssignments.RemoveRange(existingAssignments);

                // Crear nuevas asignaciones
                foreach (var assignmentDto in bulkDto.Assignments)
                {
                    // Validar que el alumno está inscrito
                    var enrollment = lesson.Enrollments.FirstOrDefault(e => e.PersonId == assignmentDto.PersonId);
                    if (enrollment == null)
                    {
                        continue; // Saltar si no está inscrito
                    }

                    // Validar que la rutina existe
                    var routine = await _context.Routines.FindAsync(assignmentDto.RoutineId);
                    if (routine == null)
                    {
                        continue; // Saltar si la rutina no existe
                    }

                    // Crear asignación
                    var assignment = new LessonRoutineAssignment
                    {
                        LessonId = bulkDto.LessonId,
                        PersonId = assignmentDto.PersonId,
                        RoutineId = assignmentDto.RoutineId,
                        AssignedAt = DateTime.UtcNow,
                        AssignedByTeacherId = teacherId
                    };

                    _context.LessonRoutineAssignments.Add(assignment);
                }

                await _context.SaveChangesAsync();

                // Obtener las asignaciones creadas
                var savedAssignments = await _context.LessonRoutineAssignments
                    .Include(a => a.Person)
                    .Include(a => a.Routine)
                    .Where(a => a.LessonId == bulkDto.LessonId)
                    .ToListAsync();

                resultAssignments = savedAssignments.Select(a => new LessonRoutineAssignmentDto
                {
                    Id = a.Id,
                    LessonId = a.LessonId,
                    PersonId = a.PersonId,
                    PersonName = a.Person.Name,
                    PersonSurname = a.Person.Surname,
                    RoutineId = a.RoutineId,
                    RoutineCategory = a.Routine.Category,
                    RoutineDescription = a.Routine.Description,
                    AssignedAt = a.AssignedAt
                }).ToList();

                return ResponseMessage<List<LessonRoutineAssignmentDto>>.SuccessResult(
                    resultAssignments,
                    $"Se asignaron {resultAssignments.Count} rutinas exitosamente"
                );
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<LessonRoutineAssignmentDto>>.Error($"Error al asignar rutinas: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<LessonRoutineAssignmentDto>> AssignRoutineAsync(LessonRoutineAssignmentCreateDto createDto, int lessonId, int teacherId)
        {
            try
            {
                var lesson = await _context.Lessons
                    .Include(l => l.Enrollments)
                    .FirstOrDefaultAsync(l => l.Id == lessonId);

                if (lesson == null)
                {
                    return ResponseMessage<LessonRoutineAssignmentDto>.NotFound("Clase no encontrada");
                }

                if (lesson.TeacherId != teacherId)
                {
                    return ResponseMessage<LessonRoutineAssignmentDto>.Error("No tiene permisos para asignar rutinas en esta clase");
                }

                // Validar que el alumno está inscrito
                var enrollment = lesson.Enrollments.FirstOrDefault(e => e.PersonId == createDto.PersonId);
                if (enrollment == null)
                {
                    return ResponseMessage<LessonRoutineAssignmentDto>.Error("El alumno no está inscrito en esta clase");
                }

                // Validar que la rutina existe
                var routine = await _context.Routines.FindAsync(createDto.RoutineId);
                if (routine == null)
                {
                    return ResponseMessage<LessonRoutineAssignmentDto>.NotFound("Rutina no encontrada");
                }

                // Eliminar asignación existente si hay
                var existing = await _context.LessonRoutineAssignments
                    .FirstOrDefaultAsync(a => a.LessonId == lessonId && a.PersonId == createDto.PersonId);
                
                if (existing != null)
                {
                    _context.LessonRoutineAssignments.Remove(existing);
                }

                // Crear nueva asignación
                var assignment = new LessonRoutineAssignment
                {
                    LessonId = lessonId,
                    PersonId = createDto.PersonId,
                    RoutineId = createDto.RoutineId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedByTeacherId = teacherId
                };

                _context.LessonRoutineAssignments.Add(assignment);
                await _context.SaveChangesAsync();

                await _context.Entry(assignment)
                    .Reference(a => a.Person)
                    .LoadAsync();
                await _context.Entry(assignment)
                    .Reference(a => a.Routine)
                    .LoadAsync();

                var dto = new LessonRoutineAssignmentDto
                {
                    Id = assignment.Id,
                    LessonId = assignment.LessonId,
                    PersonId = assignment.PersonId,
                    PersonName = assignment.Person.Name,
                    PersonSurname = assignment.Person.Surname,
                    RoutineId = assignment.RoutineId,
                    RoutineCategory = assignment.Routine.Category,
                    RoutineDescription = assignment.Routine.Description,
                    AssignedAt = assignment.AssignedAt
                };

                return ResponseMessage<LessonRoutineAssignmentDto>.SuccessResult(dto, "Rutina asignada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<LessonRoutineAssignmentDto>.Error($"Error al asignar rutina: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<object>> RemoveAssignmentAsync(int assignmentId, int teacherId)
        {
            try
            {
                var assignment = await _context.LessonRoutineAssignments
                    .Include(a => a.Lesson)
                    .FirstOrDefaultAsync(a => a.Id == assignmentId);

                if (assignment == null)
                {
                    return ResponseMessage<object>.NotFound("Asignación no encontrada");
                }

                if (assignment.Lesson.TeacherId != teacherId)
                {
                    return ResponseMessage<object>.Error("No tiene permisos para eliminar esta asignación");
                }

                _context.LessonRoutineAssignments.Remove(assignment);
                await _context.SaveChangesAsync();

                return ResponseMessage<object>.SuccessResult(null, "Asignación eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<object>.Error($"Error al eliminar asignación: {ex.Message}");
            }
        }
    }
}

