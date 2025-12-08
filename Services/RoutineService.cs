using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models.Class;
using padelya_api.Shared;
using padelya_api.models;
using padelya_api.Models;

namespace padelya_api.Services
{
    public class RoutineService : IRoutineService
    {
        private readonly PadelYaDbContext _context;

        public RoutineService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage<RoutineDto>> CreateRoutineAsync(RoutineCreateDto createDto, int teacherId)
        {
            try
            {
                // Validar que el profesor existe
                var teacher = await _context.Teachers.FindAsync(teacherId);
                if (teacher == null)
                {
                    return ResponseMessage<RoutineDto>.Error("Profesor no encontrado");
                }

                // Validar que los jugadores existen
                var players = await _context.Players
                    .Where(p => createDto.PlayerIds.Contains(p.Id))
                    .ToListAsync();

                if (players.Count != createDto.PlayerIds.Count)
                {
                    return ResponseMessage<RoutineDto>.Error("Uno o más jugadores no encontrados");
                }

                // Validar que los ejercicios existen
                var exercises = await _context.Exercises
                    .Where(e => createDto.ExerciseIds.Contains(e.Id))
                    .ToListAsync();

                if (exercises.Count != createDto.ExerciseIds.Count)
                {
                    return ResponseMessage<RoutineDto>.Error("Uno o más ejercicios no encontrados");
                }

                var routine = new Routine
                {
                    Duration = TimeOnly.FromTimeSpan(createDto.Duration),
                    Category = createDto.Category,
                    Description = createDto.Description,
                    CreatorId = teacherId,
                    Players = players,
                    Exercises = exercises
                };

                _context.Routines.Add(routine);
                await _context.SaveChangesAsync();

                var dto = await MapToDtoAsync(routine);
                return ResponseMessage<RoutineDto>.SuccessResult(dto, "Rutina creada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<RoutineDto>.Error($"Error al crear rutina: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<RoutineDto>> UpdateRoutineAsync(int routineId, RoutineUpdateDto updateDto, int teacherId)
        {
            try
            {
                var routine = await _context.Routines
                    .Include(r => r.Players)
                    .Include(r => r.Exercises)
                    .FirstOrDefaultAsync(r => r.Id == routineId);

                if (routine == null)
                {
                    return ResponseMessage<RoutineDto>.NotFound("Rutina no encontrada");
                }

                // Validar que el profesor es el creador
                if (routine.CreatorId != teacherId)
                {
                    return ResponseMessage<RoutineDto>.Error("No tiene permisos para modificar esta rutina");
                }

                // Actualizar campos
                if (updateDto.Duration.HasValue)
                    routine.Duration = TimeOnly.FromTimeSpan(updateDto.Duration.Value);
                if (!string.IsNullOrEmpty(updateDto.Category))
                    routine.Category = updateDto.Category;
                if (!string.IsNullOrEmpty(updateDto.Description))
                    routine.Description = updateDto.Description;

                // Actualizar jugadores si se proporcionan
                if (updateDto.PlayerIds != null)
                {
                    var players = await _context.Players
                        .Where(p => updateDto.PlayerIds.Contains(p.Id))
                        .ToListAsync();

                    if (players.Count != updateDto.PlayerIds.Count)
                    {
                        return ResponseMessage<RoutineDto>.Error("Uno o más jugadores no encontrados");
                    }

                    // Limpiar y agregar nuevos jugadores
                    routine.Players.Clear();
                    routine.Players.AddRange(players);
                }

                // Actualizar ejercicios si se proporcionan
                if (updateDto.ExerciseIds != null)
                {
                    var exercises = await _context.Exercises
                        .Where(e => updateDto.ExerciseIds.Contains(e.Id))
                        .ToListAsync();

                    if (exercises.Count != updateDto.ExerciseIds.Count)
                    {
                        return ResponseMessage<RoutineDto>.Error("Uno o más ejercicios no encontrados");
                    }

                    // Limpiar y agregar nuevos ejercicios
                    routine.Exercises.Clear();
                    routine.Exercises.AddRange(exercises);
                }

                _context.Routines.Update(routine);
                await _context.SaveChangesAsync();

                var dto = await MapToDtoAsync(routine);
                return ResponseMessage<RoutineDto>.SuccessResult(dto, "Rutina actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<RoutineDto>.Error($"Error al actualizar rutina: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<RoutineDto>> GetRoutineByIdAsync(int routineId)
        {
            try
            {
                var routine = await _context.Routines
                    .Include(r => r.Players)
                    .Include(r => r.Exercises)
                    .Include(r => r.Creator)
                    .FirstOrDefaultAsync(r => r.Id == routineId);

                if (routine == null)
                {
                    return ResponseMessage<RoutineDto>.NotFound("Rutina no encontrada");
                }

                var dto = await MapToDtoAsync(routine);
                return ResponseMessage<RoutineDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return ResponseMessage<RoutineDto>.Error($"Error al obtener rutina: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<RoutineDto>>> GetRoutinesByTeacherAsync(int teacherId)
        {
            try
            {
                var routines = await _context.Routines
                    .Include(r => r.Players)
                    .Include(r => r.Exercises)
                    .Include(r => r.Creator)
                    .Where(r => r.CreatorId == teacherId)
                    .OrderByDescending(r => r.Id)
                    .ToListAsync();

                var dtos = new List<RoutineDto>();
                foreach (var routine in routines)
                {
                    dtos.Add(await MapToDtoAsync(routine));
                }

                return ResponseMessage<List<RoutineDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<RoutineDto>>.Error($"Error al obtener rutinas: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<RoutineDto>>> GetRoutinesByPlayerAsync(int playerId)
        {
            try
            {
                var routines = await _context.Routines
                    .Include(r => r.Players)
                    .Include(r => r.Exercises)
                    .Include(r => r.Creator)
                    .Where(r => r.Players.Any(p => p.Id == playerId))
                    .OrderByDescending(r => r.Id)
                    .ToListAsync();

                var dtos = new List<RoutineDto>();
                foreach (var routine in routines)
                {
                    dtos.Add(await MapToDtoAsync(routine));
                }

                return ResponseMessage<List<RoutineDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<RoutineDto>>.Error($"Error al obtener rutinas: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<bool>> DeleteRoutineAsync(int routineId, int teacherId)
        {
            try
            {
                var routine = await _context.Routines.FindAsync(routineId);
                if (routine == null)
                {
                    return ResponseMessage<bool>.NotFound("Rutina no encontrada");
                }

                if (routine.CreatorId != teacherId)
                {
                    return ResponseMessage<bool>.Error("No tiene permisos para eliminar esta rutina");
                }

                _context.Routines.Remove(routine);
                await _context.SaveChangesAsync();

                return ResponseMessage<bool>.SuccessResult(true, "Rutina eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<bool>.Error($"Error al eliminar rutina: {ex.Message}");
            }
        }

        private async Task<RoutineDto> MapToDtoAsync(Routine routine)
        {
            // Cargar relaciones si no están cargadas
            if (!_context.Entry(routine).Collection(r => r.Players).IsLoaded)
            {
                await _context.Entry(routine).Collection(r => r.Players).LoadAsync();
            }

            if (!_context.Entry(routine).Collection(r => r.Exercises).IsLoaded)
            {
                await _context.Entry(routine).Collection(r => r.Exercises).LoadAsync();
            }

            var creator = await _context.Users
                .FirstOrDefaultAsync(u => u.PersonId == routine.CreatorId);

            var playerIds = routine.Players?.Select(p => p.Id).ToList() ?? new List<int>();
            var playerUsers = playerIds.Count > 0
                ? await _context.Users
                    .Where(u => u.PersonId.HasValue && playerIds.Contains(u.PersonId.Value))
                    .ToListAsync()
                : new List<Models.User>();

            return new RoutineDto
            {
                Id = routine.Id,
                Duration = routine.Duration.ToTimeSpan(),
                Category = routine.Category,
                Description = routine.Description,
                CreatorId = routine.CreatorId,
                CreatorName = creator?.Name,
                PlayerIds = playerIds,
                PlayerNames = playerUsers.Select(u => $"{u.Name} {u.Surname}").ToList(),
                ExerciseIds = routine.Exercises?.Select(e => e.Id).ToList() ?? new List<int>(),
                Exercises = routine.Exercises?.Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description ?? string.Empty,
                    Category = e.Category ?? string.Empty
                }).ToList() ?? new List<ExerciseDto>()
            };
        }
    }
}

