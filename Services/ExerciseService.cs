using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models.Class;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly PadelYaDbContext _context;

        public ExerciseService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage<List<ExerciseDto>>> GetAllExercisesAsync()
        {
            try
            {
                var exercises = await _context.Exercises
                    .OrderBy(e => e.Category)
                    .ThenBy(e => e.Name)
                    .ToListAsync();

                var dtos = exercises.Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Category = e.Category
                }).ToList();

                return ResponseMessage<List<ExerciseDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<ExerciseDto>>.Error($"Error al obtener ejercicios: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<ExerciseDto>> GetExerciseByIdAsync(int exerciseId)
        {
            try
            {
                var exercise = await _context.Exercises.FindAsync(exerciseId);
                
                if (exercise == null)
                {
                    return ResponseMessage<ExerciseDto>.NotFound("Ejercicio no encontrado");
                }

                var dto = new ExerciseDto
                {
                    Id = exercise.Id,
                    Name = exercise.Name,
                    Description = exercise.Description,
                    Category = exercise.Category
                };

                return ResponseMessage<ExerciseDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return ResponseMessage<ExerciseDto>.Error($"Error al obtener ejercicio: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<ExerciseDto>> CreateExerciseAsync(ExerciseCreateDto createDto)
        {
            try
            {
                var exercise = new Exercise
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Category = createDto.Category
                };

                _context.Exercises.Add(exercise);
                await _context.SaveChangesAsync();

                var dto = new ExerciseDto
                {
                    Id = exercise.Id,
                    Name = exercise.Name,
                    Description = exercise.Description,
                    Category = exercise.Category
                };

                return ResponseMessage<ExerciseDto>.SuccessResult(dto, "Ejercicio creado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<ExerciseDto>.Error($"Error al crear ejercicio: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<ExerciseDto>> UpdateExerciseAsync(int exerciseId, ExerciseUpdateDto updateDto)
        {
            try
            {
                var exercise = await _context.Exercises.FindAsync(exerciseId);
                
                if (exercise == null)
                {
                    return ResponseMessage<ExerciseDto>.NotFound("Ejercicio no encontrado");
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    exercise.Name = updateDto.Name;
                }
                
                if (!string.IsNullOrWhiteSpace(updateDto.Description))
                {
                    exercise.Description = updateDto.Description;
                }
                
                if (!string.IsNullOrWhiteSpace(updateDto.Category))
                {
                    exercise.Category = updateDto.Category;
                }

                await _context.SaveChangesAsync();

                var dto = new ExerciseDto
                {
                    Id = exercise.Id,
                    Name = exercise.Name,
                    Description = exercise.Description,
                    Category = exercise.Category
                };

                return ResponseMessage<ExerciseDto>.SuccessResult(dto, "Ejercicio actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<ExerciseDto>.Error($"Error al actualizar ejercicio: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<bool>> DeleteExerciseAsync(int exerciseId)
        {
            try
            {
                var exercise = await _context.Exercises.FindAsync(exerciseId);
                
                if (exercise == null)
                {
                    return ResponseMessage<bool>.NotFound("Ejercicio no encontrado");
                }

                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();

                return ResponseMessage<bool>.SuccessResult(true, "Ejercicio eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<bool>.Error($"Error al eliminar ejercicio: {ex.Message}");
            }
        }
    }
}

