using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.Models.Class;
using padelya_api.Shared;
using padelya_api.models;

namespace padelya_api.Services
{
    public class StatsService : IStatsService
    {
        private readonly PadelYaDbContext _context;

        public StatsService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage<StatsDto>> CreateStatsAsync(StatsCreateDto createDto, int teacherId)
        {
            try
            {
                // Validar que el jugador existe
                var player = await _context.Players.FindAsync(createDto.PlayerId);
                if (player == null)
                {
                    return ResponseMessage<StatsDto>.NotFound("Jugador no encontrado");
                }

                // Validar que el profesor existe
                var teacher = await _context.Teachers.FindAsync(teacherId);
                if (teacher == null)
                {
                    return ResponseMessage<StatsDto>.Error("Profesor no encontrado");
                }

                // Si se especifica una clase, validar que existe y pertenece al profesor
                if (createDto.LessonId.HasValue)
                {
                    var lesson = await _context.Lessons
                        .FirstOrDefaultAsync(l => l.Id == createDto.LessonId.Value);
                    
                    if (lesson == null)
                    {
                        return ResponseMessage<StatsDto>.NotFound("Clase no encontrada");
                    }

                    if (lesson.TeacherId != teacherId)
                    {
                        return ResponseMessage<StatsDto>.Error("No tiene permisos para registrar progreso en esta clase");
                    }
                }

                var stats = new Stats
                {
                    Drive = createDto.Drive,
                    Backhand = createDto.Backhand,
                    Smash = createDto.Smash,
                    Serve = createDto.Serve,
                    Vibora = createDto.Vibora,
                    Bandeja = createDto.Bandeja,
                    Observations = createDto.Observations,
                    Milestones = createDto.Milestones,
                    PlayerId = createDto.PlayerId,
                    LessonId = createDto.LessonId,
                    RecordedAt = DateTime.UtcNow,
                    RecordedByTeacherId = teacherId
                };

                _context.Stats.Add(stats);
                await _context.SaveChangesAsync();

                var dto = await MapToDtoAsync(stats);
                return ResponseMessage<StatsDto>.SuccessResult(dto, "Progreso registrado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<StatsDto>.Error($"Error al registrar progreso: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<StatsDto>> UpdateStatsAsync(int statsId, StatsUpdateDto updateDto, int teacherId)
        {
            try
            {
                var stats = await _context.Stats
                    .Include(s => s.Lesson)
                    .FirstOrDefaultAsync(s => s.Id == statsId);

                if (stats == null)
                {
                    return ResponseMessage<StatsDto>.NotFound("Registro de progreso no encontrado");
                }

                // Validar que el profesor tiene permisos
                if (stats.RecordedByTeacherId != teacherId)
                {
                    return ResponseMessage<StatsDto>.Error("No tiene permisos para modificar este registro");
                }

                // Actualizar campos si se proporcionan
                if (updateDto.Drive.HasValue) stats.Drive = updateDto.Drive.Value;
                if (updateDto.Backhand.HasValue) stats.Backhand = updateDto.Backhand.Value;
                if (updateDto.Smash.HasValue) stats.Smash = updateDto.Smash.Value;
                if (updateDto.Serve.HasValue) stats.Serve = updateDto.Serve.Value;
                if (updateDto.Vibora.HasValue) stats.Vibora = updateDto.Vibora.Value;
                if (updateDto.Bandeja.HasValue) stats.Bandeja = updateDto.Bandeja.Value;
                if (updateDto.Observations != null) stats.Observations = updateDto.Observations;
                if (updateDto.Milestones != null) stats.Milestones = updateDto.Milestones;

                _context.Stats.Update(stats);
                await _context.SaveChangesAsync();

                var dto = await MapToDtoAsync(stats);
                return ResponseMessage<StatsDto>.SuccessResult(dto, "Progreso actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<StatsDto>.Error($"Error al actualizar progreso: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<StatsDto>> GetStatsByIdAsync(int statsId)
        {
            try
            {
                var stats = await _context.Stats
                    .Include(s => s.Player)
                    .Include(s => s.Lesson)
                    .Include(s => s.RecordedByTeacher)
                    .FirstOrDefaultAsync(s => s.Id == statsId);

                if (stats == null)
                {
                    return ResponseMessage<StatsDto>.NotFound("Registro de progreso no encontrado");
                }

                var dto = await MapToDtoAsync(stats);
                return ResponseMessage<StatsDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return ResponseMessage<StatsDto>.Error($"Error al obtener progreso: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<StatsDto>>> GetStatsByPlayerAsync(int playerId)
        {
            try
            {
                var statsList = await _context.Stats
                    .Include(s => s.Player)
                    .Include(s => s.Lesson)
                    .ThenInclude(l => l.CourtSlot)
                    .Include(s => s.RecordedByTeacher)
                    .Where(s => s.PlayerId == playerId)
                    .OrderByDescending(s => s.RecordedAt)
                    .ToListAsync();

                var dtos = new List<StatsDto>();
                foreach (var stats in statsList)
                {
                    dtos.Add(await MapToDtoAsync(stats));
                }

                return ResponseMessage<List<StatsDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<StatsDto>>.Error($"Error al obtener progreso: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<List<StatsDto>>> GetStatsByLessonAsync(int lessonId)
        {
            try
            {
                var statsList = await _context.Stats
                    .Include(s => s.Player)
                    .Include(s => s.Lesson)
                    .Include(s => s.RecordedByTeacher)
                    .Where(s => s.LessonId == lessonId)
                    .OrderByDescending(s => s.RecordedAt)
                    .ToListAsync();

                var dtos = new List<StatsDto>();
                foreach (var stats in statsList)
                {
                    dtos.Add(await MapToDtoAsync(stats));
                }

                return ResponseMessage<List<StatsDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage<List<StatsDto>>.Error($"Error al obtener progreso: {ex.Message}");
            }
        }

        public async Task<ResponseMessage<bool>> DeleteStatsAsync(int statsId, int teacherId)
        {
            try
            {
                var stats = await _context.Stats.FindAsync(statsId);
                if (stats == null)
                {
                    return ResponseMessage<bool>.NotFound("Registro de progreso no encontrado");
                }

                if (stats.RecordedByTeacherId != teacherId)
                {
                    return ResponseMessage<bool>.Error("No tiene permisos para eliminar este registro");
                }

                _context.Stats.Remove(stats);
                await _context.SaveChangesAsync();

                return ResponseMessage<bool>.SuccessResult(true, "Registro de progreso eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ResponseMessage<bool>.Error($"Error al eliminar progreso: {ex.Message}");
            }
        }

        private async Task<StatsDto> MapToDtoAsync(Stats stats)
        {
            await _context.Entry(stats)
                .Reference(s => s.Player)
                .LoadAsync();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PersonId == stats.PlayerId);

            var teacher = stats.RecordedByTeacherId.HasValue
                ? await _context.Users
                    .FirstOrDefaultAsync(u => u.PersonId == stats.RecordedByTeacherId.Value)
                : null;

            return new StatsDto
            {
                Id = stats.Id,
                Drive = stats.Drive,
                Backhand = stats.Backhand,
                Smash = stats.Smash,
                Serve = stats.Serve,
                Vibora = stats.Vibora,
                Bandeja = stats.Bandeja,
                Observations = stats.Observations,
                Milestones = stats.Milestones,
                PlayerId = stats.PlayerId,
                PlayerName = user?.Name,
                PlayerSurname = user?.Surname,
                LessonId = stats.LessonId,
                LessonDate = stats.Lesson?.CourtSlot?.Date,
                RecordedAt = stats.RecordedAt,
                RecordedByTeacherId = stats.RecordedByTeacherId,
                TeacherName = teacher?.Name
            };
        }
    }
}

