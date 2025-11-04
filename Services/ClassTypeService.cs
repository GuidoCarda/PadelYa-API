using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public class ClassTypeService : IClassTypeService
    {
        private readonly PadelYaDbContext _context;

        public ClassTypeService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage<List<ClassTypeDto>>> GetAllAsync()
        {
            try
            {
                var classTypes = await _context.ClassTypes
                    .OrderBy(ct => ct.Name)
                    .ToListAsync();

                // Count lessons by ClassType string (temporary until migration creates ClassTypeId)
                var lessonsCounts = await _context.Lessons
                    .Where(l => l.ClassType != null)
                    .GroupBy(l => l.ClassType)
                    .ToDictionaryAsync(g => g.Key!, g => g.Count());

                var dtos = classTypes.Select(ct => new ClassTypeDto
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description,
                    Level = ct.Level,
                    CreatedAt = ct.CreatedAt,
                    UpdatedAt = ct.UpdatedAt,
                    LessonsCount = lessonsCounts.GetValueOrDefault(ct.Name, 0)
                }).ToList();

                return ResponseMessage<List<ClassTypeDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<ClassTypeDto>>.Error($"Error al obtener los tipos de clase: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<ClassTypeDto>> GetByIdAsync(int id)
        {
            try
            {
                var classType = await _context.ClassTypes
                    .FirstOrDefaultAsync(ct => ct.Id == id);

                if (classType == null)
                {
                    return ResponseMessage<ClassTypeDto>.NotFound("Tipo de clase no encontrado");
                }

                // Count lessons by ClassType string (temporary until migration creates ClassTypeId)
                var lessonsCount = await _context.Lessons
                    .CountAsync(l => l.ClassType == classType.Name);

                var dto = new ClassTypeDto
                {
                    Id = classType.Id,
                    Name = classType.Name,
                    Description = classType.Description,
                    Level = classType.Level,
                    CreatedAt = classType.CreatedAt,
                    UpdatedAt = classType.UpdatedAt,
                    LessonsCount = lessonsCount
                };

                return ResponseMessage<ClassTypeDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return ResponseMessage<ClassTypeDto>.Error($"Error al obtener el tipo de clase: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<ClassTypeDto>> CreateAsync(ClassTypeCreateDto createDto)
        {
            try
            {
                // Verificar si ya existe un tipo con el mismo nombre
                var existing = await _context.ClassTypes
                    .FirstOrDefaultAsync(ct => ct.Name.ToLower() == createDto.Name.ToLower());

                if (existing != null)
                {
                    return ResponseMessage<ClassTypeDto>.Error("Ya existe un tipo de clase con ese nombre");
                }

                var classType = new ClassType
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Level = createDto.Level,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ClassTypes.Add(classType);
                await _context.SaveChangesAsync();

                var dto = new ClassTypeDto
                {
                    Id = classType.Id,
                    Name = classType.Name,
                    Description = classType.Description,
                    Level = classType.Level,
                    CreatedAt = classType.CreatedAt,
                    UpdatedAt = classType.UpdatedAt,
                    LessonsCount = 0
                };

                return ResponseMessage<ClassTypeDto>.SuccessResult(dto, "Tipo de clase creado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<ClassTypeDto>.Error($"Error al crear el tipo de clase: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<ClassTypeDto>> UpdateAsync(ClassTypeUpdateDto updateDto)
        {
            try
            {
                var classType = await _context.ClassTypes
                    .FirstOrDefaultAsync(ct => ct.Id == updateDto.Id);

                if (classType == null)
                {
                    return ResponseMessage<ClassTypeDto>.NotFound("Tipo de clase no encontrado");
                }

                // Verificar si ya existe otro tipo con el mismo nombre (excluyendo el actual)
                var existing = await _context.ClassTypes
                    .FirstOrDefaultAsync(ct => ct.Name.ToLower() == updateDto.Name.ToLower() && ct.Id != updateDto.Id);

                if (existing != null)
                {
                    return ResponseMessage<ClassTypeDto>.Error("Ya existe otro tipo de clase con ese nombre");
                }

                classType.Name = updateDto.Name;
                classType.Description = updateDto.Description;
                classType.Level = updateDto.Level;
                classType.UpdatedAt = DateTime.UtcNow;

                _context.ClassTypes.Update(classType);
                await _context.SaveChangesAsync();

                // Count lessons by ClassType string (temporary until migration creates ClassTypeId)
                var lessonsCount = await _context.Lessons
                    .CountAsync(l => l.ClassType == classType.Name);

                var dto = new ClassTypeDto
                {
                    Id = classType.Id,
                    Name = classType.Name,
                    Description = classType.Description,
                    Level = classType.Level,
                    CreatedAt = classType.CreatedAt,
                    UpdatedAt = classType.UpdatedAt,
                    LessonsCount = lessonsCount
                };

                return ResponseMessage<ClassTypeDto>.SuccessResult(dto, "Tipo de clase actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<ClassTypeDto>.Error($"Error al actualizar el tipo de clase: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<bool>> DeleteAsync(int id)
        {
            try
            {
                var classType = await _context.ClassTypes
                    .FirstOrDefaultAsync(ct => ct.Id == id);

                if (classType == null)
                {
                    return ResponseMessage<bool>.NotFound("Tipo de clase no encontrado");
                }

                // Verificar si tiene clases programadas (temporary check using ClassType string)
                var activeLessons = await _context.Lessons
                    .Include(l => l.CourtSlot)
                    .Include(l => l.Enrollments)
                    .Where(l => l.ClassType == classType.Name)
                    .ToListAsync();

                if (activeLessons.Any(l => !l.HasEnded))
                {
                    return ResponseMessage<bool>.Error(
                        "No se puede eliminar un tipo de clase que tiene clases programadas activas");
                }

                // Verificar si tiene clases con alumnos inscritos
                var hasEnrollments = activeLessons.Any(l => l.Enrollments != null && l.Enrollments.Any());
                if (hasEnrollments)
                {
                    return ResponseMessage<bool>.Error(
                        "No se puede eliminar un tipo de clase que tiene clases con alumnos inscritos");
                }

                _context.ClassTypes.Remove(classType);
                await _context.SaveChangesAsync();

                return ResponseMessage<bool>.SuccessResult(true, "Tipo de clase eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<bool>.Error($"Error al eliminar el tipo de clase: {ex.Message}");
            }
        }
    }
}

