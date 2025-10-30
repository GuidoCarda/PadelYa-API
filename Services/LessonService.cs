using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.models;
using padelya_api.Models;
using padelya_api.Shared;

namespace padelya_api.Services
{
  public class LessonService : ILessonService
  {
    private readonly PadelYaDbContext _context;
    private readonly ICourtSlotService _courtSlotService;

    public LessonService(PadelYaDbContext context, ICourtSlotService courtSlotService)
    {
      _context = context;
      _courtSlotService = courtSlotService;
    }

    public async Task<ResponseMessage<LessonResponseDto>> CreateLessonAsync(LessonCreateDto createDto)
    {
      try
      {
        // Validar que el profesor exista
        var teacher = await _context.Teachers.FindAsync(createDto.TeacherId);
        if (teacher == null)
        {
          return ResponseMessage<LessonResponseDto>.Error("El profesor especificado no existe");
        }

        // Validar que la cancha exista
        var court = await _context.Courts.FindAsync(createDto.CourtId);
        if (court == null)
        {
          return ResponseMessage<LessonResponseDto>.Error("La cancha especificada no existe");
        }

        // Validar horarios de la cancha
        // Special handling: 00:00 as closing time means end of day (24:00)
        var effectiveClosingTime = court.ClosingTime == TimeOnly.MinValue
            ? new TimeOnly(23, 59, 59)
            : court.ClosingTime;

        if (createDto.StartTime < court.OpeningTime || createDto.EndTime > effectiveClosingTime)
        {
          return ResponseMessage<LessonResponseDto>.Error(
              $"El horario debe estar dentro del horario de la cancha ({court.OpeningTime} - {effectiveClosingTime})");
        }

        // Validar que la hora de inicio sea antes que la de fin
        if (createDto.StartTime >= createDto.EndTime)
        {
          return ResponseMessage<LessonResponseDto>.Error("La hora de inicio debe ser anterior a la hora de fin");
        }

        // Validar que la fecha no sea en el pasado
        if (createDto.Date.Date < DateTime.Today)
        {
          return ResponseMessage<LessonResponseDto>.Error("No se puede programar una clase en el pasado");
        }

        // Verificar disponibilidad de la cancha
        var availabilityCheck = await ValidateCourtAvailabilityAsync(
            createDto.CourtId, createDto.Date, createDto.StartTime, createDto.EndTime);

        if (!availabilityCheck.Success)
        {
          return ResponseMessage<LessonResponseDto>.Error(availabilityCheck.Message);
        }

        // Crear CourtSlot
        var courtSlot = new CourtSlot
        {
          CourtId = createDto.CourtId,
          Date = createDto.Date,
          StartTime = createDto.StartTime,
          EndTime = createDto.EndTime
        };

        _context.CourtSlots.Add(courtSlot);
        await _context.SaveChangesAsync();

        // Crear Lesson
        var lesson = new Lesson
        {
          Price = createDto.Price,
          MaxCapacity = createDto.MaxCapacity,
          Description = createDto.Description,
          ClassType = createDto.ClassType,
          TeacherId = createDto.TeacherId,
          CourtSlotId = courtSlot.Id,
          CreatedAt = DateTime.UtcNow
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        // Cargar entidades relacionadas para la respuesta
        await _context.Entry(lesson)
            .Reference(l => l.Teacher)
            .LoadAsync();

        await _context.Entry(lesson)
            .Reference(l => l.CourtSlot)
            .LoadAsync();

        await _context.Entry(lesson.CourtSlot)
            .Reference(cs => cs.Court)
            .LoadAsync();

        var responseDto = MapToLessonResponseDto(lesson);
        return ResponseMessage<LessonResponseDto>.SuccessResult(responseDto, "Clase creada exitosamente");
      }
      catch (Exception ex)
      {
        return ResponseMessage<LessonResponseDto>.Error($"Error al crear la clase: {ex.Message}");
      }
    }

    public async Task<ResponseMessage<List<LessonResponseDto>>> CreateRecurrentLessonsAsync(LessonCreateDto createDto)
    {
      if (!createDto.IsRecurrent)
      {
        var singleLesson = await CreateLessonAsync(createDto);
        if (singleLesson.Success)
        {
          return ResponseMessage<List<LessonResponseDto>>.SuccessResult(
              new List<LessonResponseDto> { singleLesson.Data! },
              "Clase creada exitosamente");
        }
        return ResponseMessage<List<LessonResponseDto>>.Error(singleLesson.Message);
      }

      if (createDto.RecurrenceEndDate == null)
      {
        return ResponseMessage<List<LessonResponseDto>>.Error(
            "La fecha de fin de recurrencia es obligatoria para clases recurrentes");
      }

      var lessons = new List<LessonResponseDto>();
      var currentDate = createDto.Date;
      var endDate = createDto.RecurrenceEndDate.Value;

      try
      {
        while (currentDate <= endDate)
        {
          var lessonDto = new LessonCreateDto
          {
            Price = createDto.Price,
            TeacherId = createDto.TeacherId,
            CourtId = createDto.CourtId,
            Date = currentDate,
            StartTime = createDto.StartTime,
            EndTime = createDto.EndTime,
            MaxCapacity = createDto.MaxCapacity,
            Description = createDto.Description,
            ClassType = createDto.ClassType,
            IsRecurrent = false
          };

          var result = await CreateLessonAsync(lessonDto);
          if (result.Success)
          {
            lessons.Add(result.Data!);
          }

          // Calcular siguiente fecha según el patrón
          currentDate = CalculateNextDate(currentDate, createDto.RecurrencePattern!,
              createDto.RecurrenceInterval ?? 1, createDto.WeeklyDays);
        }

        return ResponseMessage<List<LessonResponseDto>>.SuccessResult(lessons,
            $"Se crearon {lessons.Count} clases recurrentes exitosamente");
      }
      catch (Exception ex)
      {
        return ResponseMessage<List<LessonResponseDto>>.Error(
            $"Error al crear clases recurrentes: {ex.Message}");
      }
    }

    public async Task<ResponseMessage<LessonResponseDto>> GetLessonByIdAsync(int id)
    {
      try
      {
        var lesson = await _context.Lessons
            .Include(l => l.Teacher)
            .Include(l => l.CourtSlot)
                .ThenInclude(cs => cs.Court)
            .Include(l => l.Enrollments)
                .ThenInclude(e => e.Person)
            .Include(l => l.Enrollments)
                .ThenInclude(e => e.Payment)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
          return ResponseMessage<LessonResponseDto>.NotFound("Clase no encontrada");
        }

        var responseDto = MapToLessonResponseDto(lesson);
        return ResponseMessage<LessonResponseDto>.SuccessResult(responseDto);
      }
      catch (Exception ex)
      {
        return ResponseMessage<LessonResponseDto>.Error($"Error al obtener la clase: {ex.Message}");
      }
    }

    public async Task<ResponseMessage<List<LessonListDto>>> GetLessonsAsync(LessonFilterDto filterDto)
    {
      try
      {
        var query = _context.Lessons
            .Include(l => l.Teacher)
            .Include(l => l.CourtSlot)
                .ThenInclude(cs => cs.Court)
            .Include(l => l.Enrollments)
            .AsQueryable();

        // Buscar el usuario asociado al teacher a través de PersonId
        var lessonsWithUsers = from lesson in query
                               join user in _context.Users on lesson.Teacher.Id equals user.PersonId into userGroup
                               from user in userGroup.DefaultIfEmpty()
                               select new { lesson, user };

        // Aplicar filtros
        if (filterDto.StartDate.HasValue)
        {
          lessonsWithUsers = lessonsWithUsers.Where(x => x.lesson.CourtSlot.Date >= filterDto.StartDate.Value);
        }

        if (filterDto.EndDate.HasValue)
        {
          lessonsWithUsers = lessonsWithUsers.Where(x => x.lesson.CourtSlot.Date <= filterDto.EndDate.Value);
        }

        if (filterDto.TeacherId.HasValue)
        {
          lessonsWithUsers = lessonsWithUsers.Where(x => x.lesson.TeacherId == filterDto.TeacherId.Value);
        }

        if (filterDto.CourtId.HasValue)
        {
          lessonsWithUsers = lessonsWithUsers.Where(x => x.lesson.CourtSlot.CourtId == filterDto.CourtId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filterDto.ClassType))
        {
          lessonsWithUsers = lessonsWithUsers.Where(x => x.lesson.ClassType!.Contains(filterDto.ClassType));
        }

        if (filterDto.AvailableOnly == true)
        {
          lessonsWithUsers = lessonsWithUsers.Where(x => x.lesson.Enrollments.Count < x.lesson.MaxCapacity);
        }

        var orderedResults = lessonsWithUsers.OrderBy(x => x.lesson.CourtSlot.Date).ThenBy(x => x.lesson.CourtSlot.StartTime);

        var results = await orderedResults.ToListAsync();

        var lessonDtos = results.Select(r => MapToLessonListDto(r.lesson, r.user)).ToList();

        return ResponseMessage<List<LessonListDto>>.SuccessResult(lessonDtos);
      }
      catch (Exception ex)
      {
        return ResponseMessage<List<LessonListDto>>.Error($"Error al obtener las clases: {ex.Message}");
      }
    }

    public async Task<ResponseMessage<LessonResponseDto>> UpdateLessonAsync(LessonUpdateDto updateDto)
    {
      try
      {
        var lesson = await _context.Lessons
            .Include(l => l.CourtSlot)
            .FirstOrDefaultAsync(l => l.Id == updateDto.Id);

        if (lesson == null)
        {
          return ResponseMessage<LessonResponseDto>.NotFound("Clase no encontrada");
        }

        // No permitir modificar clases que ya comenzaron
        if (lesson.HasStarted)
        {
          return ResponseMessage<LessonResponseDto>.Error(
              "No se puede modificar una clase que ya ha comenzado");
        }

        // Actualizar campos de la lesson
        if (updateDto.Price.HasValue)
          lesson.Price = updateDto.Price.Value;

        if (updateDto.MaxCapacity.HasValue)
          lesson.MaxCapacity = updateDto.MaxCapacity.Value;

        if (updateDto.Description != null)
          lesson.Description = updateDto.Description;

        if (updateDto.ClassType != null)
          lesson.ClassType = updateDto.ClassType;

        if (updateDto.TeacherId.HasValue)
        {
          var teacher = await _context.Teachers.FindAsync(updateDto.TeacherId.Value);
          if (teacher == null)
          {
            return ResponseMessage<LessonResponseDto>.Error("El profesor especificado no existe");
          }
          lesson.TeacherId = updateDto.TeacherId.Value;
        }

        // Si se actualiza información del slot de cancha
        bool courtSlotChanged = false;
        if (updateDto.CourtId.HasValue || updateDto.Date.HasValue ||
            updateDto.StartTime.HasValue || updateDto.EndTime.HasValue)
        {
          var newCourtId = updateDto.CourtId ?? lesson.CourtSlot.CourtId;
          var newDate = updateDto.Date ?? lesson.CourtSlot.Date;
          var newStartTime = updateDto.StartTime ?? lesson.CourtSlot.StartTime;
          var newEndTime = updateDto.EndTime ?? lesson.CourtSlot.EndTime;

          // Validar disponibilidad
          var availabilityCheck = await ValidateCourtAvailabilityAsync(
              newCourtId, newDate, newStartTime, newEndTime, lesson.Id);

          if (!availabilityCheck.Success)
          {
            return ResponseMessage<LessonResponseDto>.Error(availabilityCheck.Message);
          }

          lesson.CourtSlot.CourtId = newCourtId;
          lesson.CourtSlot.Date = newDate;
          lesson.CourtSlot.StartTime = newStartTime;
          lesson.CourtSlot.EndTime = newEndTime;
          courtSlotChanged = true;
        }

        lesson.UpdatedAt = DateTime.UtcNow;

        if (courtSlotChanged)
        {
          _context.CourtSlots.Update(lesson.CourtSlot);
        }

        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();

        // Recargar con relaciones para la respuesta
        await _context.Entry(lesson)
            .Reference(l => l.Teacher)
            .LoadAsync();

        await _context.Entry(lesson.CourtSlot)
            .Reference(cs => cs.Court)
            .LoadAsync();

        var responseDto = MapToLessonResponseDto(lesson);
        return ResponseMessage<LessonResponseDto>.SuccessResult(responseDto, "Clase actualizada exitosamente");
      }
      catch (Exception ex)
      {
        return ResponseMessage<LessonResponseDto>.Error($"Error al actualizar la clase: {ex.Message}");
      }
    }

    public async Task<ResponseMessage<bool>> DeleteLessonAsync(int id)
    {
      try
      {
        var lesson = await _context.Lessons
            .Include(l => l.CourtSlot)
            .Include(l => l.Enrollments)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
          return ResponseMessage<bool>.NotFound("Clase no encontrada");
        }

        if (lesson.HasStarted)
        {
          return ResponseMessage<bool>.Error("No se puede eliminar una clase que ya ha comenzado");
        }

        if (lesson.Enrollments.Any())
        {
          return ResponseMessage<bool>.Error(
              "No se puede eliminar una clase que tiene inscripciones. Considere cancelarla en su lugar.");
        }

        _context.Lessons.Remove(lesson);
        _context.CourtSlots.Remove(lesson.CourtSlot);
        await _context.SaveChangesAsync();

        return ResponseMessage<bool>.SuccessResult(true, "Clase eliminada exitosamente");
      }
      catch (Exception ex)
      {
        return ResponseMessage<bool>.Error($"Error al eliminar la clase: {ex.Message}");
      }
    }

    public async Task<ResponseMessage<bool>> CancelLessonAsync(int id)
    {
      try
      {
        var lesson = await _context.Lessons
            .Include(l => l.Enrollments)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
          return ResponseMessage<bool>.NotFound("Clase no encontrada");
        }

        if (lesson.HasEnded)
        {
          return ResponseMessage<bool>.Error("No se puede cancelar una clase que ya terminó");
        }

        // Aquí podrías agregar lógica para manejar reembolsos, notificaciones, etc.
        lesson.UpdatedAt = DateTime.UtcNow;
        // Nota: Podrías agregar un campo Status en el modelo para manejar estados

        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();

        return ResponseMessage<bool>.SuccessResult(true, "Clase cancelada exitosamente");
      }
      catch (Exception ex)
      {
        return ResponseMessage<bool>.Error($"Error al cancelar la clase: {ex.Message}");
      }
    }

    public async Task<ResponseMessage<List<LessonListDto>>> GetLessonsByTeacherAsync(int teacherId, DateTime? startDate = null, DateTime? endDate = null)
    {
      var filterDto = new LessonFilterDto { TeacherId = teacherId, StartDate = startDate, EndDate = endDate };
      return await GetLessonsAsync(filterDto);
    }

    public async Task<ResponseMessage<List<LessonListDto>>> GetLessonsByCourtAsync(int courtId, DateTime? startDate = null, DateTime? endDate = null)
    {
      var filterDto = new LessonFilterDto { CourtId = courtId, StartDate = startDate, EndDate = endDate };
      return await GetLessonsAsync(filterDto);
    }

    public async Task<ResponseMessage<bool>> ValidateCourtAvailabilityAsync(
        int courtId, DateTime date, TimeOnly startTime, TimeOnly endTime, int? excludeLessonId = null)
    {
      try
      {
        var conflictingSlots = await _context.CourtSlots
            .Where(cs => cs.CourtId == courtId &&
                        cs.Date.Date == date.Date &&
                        cs.StartTime == startTime &&
                        cs.EndTime == endTime &&
                        (cs.Status == CourtSlotStatus.Active ||
                        cs.Status == CourtSlotStatus.Pending))
            .Include(cs => cs.Lesson)
            .Include(cs => cs.Booking)
            .Include(cs => cs.TournamentMatch)
            .ToListAsync();



        if (excludeLessonId.HasValue)
        {
          conflictingSlots = conflictingSlots
              .Where(cs => cs.Lesson?.Id != excludeLessonId.Value)
              .ToList();
        }

        if (conflictingSlots.Any())
        {
          var conflict = conflictingSlots.First();
          string conflictType = conflict.Lesson != null ? "clase" :
                               conflict.Booking != null ? "reserva" : "torneo";

          return ResponseMessage<bool>.Error(
              $"La cancha no está disponible en ese horario. Hay una {conflictType} programada de {conflict.StartTime} a {conflict.EndTime}");
        }

        return ResponseMessage<bool>.SuccessResult(true);
      }
      catch (Exception ex)
      {
        return ResponseMessage<bool>.Error($"Error al verificar disponibilidad: {ex.Message}");
      }
    }

    public async Task<ResponseMessage<List<object>>> GetTeachersAsync()
    {
      try
      {
        var teachers = await _context.Users
            .Where(u => u.Person != null && u.Person is Teacher)
            .Select(u => new
            {
              id = u.PersonId,
              name = u.Name,
              surname = u.Surname,
              email = u.Email
            })
            .ToListAsync();

        var teacherObjects = teachers.Cast<object>().ToList();
        return ResponseMessage<List<object>>.SuccessResult(teacherObjects);
      }
      catch (Exception ex)
      {
        return ResponseMessage<List<object>>.Error($"Error al obtener profesores: {ex.Message}");
      }
    }

    #region Helper Methods

    private DateTime CalculateNextDate(DateTime currentDate, string pattern, int interval, List<DayOfWeek>? weeklyDays)
    {
      return pattern.ToLower() switch
      {
        "daily" => currentDate.AddDays(interval),
        "weekly" => currentDate.AddDays(7 * interval),
        "monthly" => currentDate.AddMonths(interval),
        _ => currentDate.AddDays(interval)
      };
    }

    private LessonResponseDto MapToLessonResponseDto(Lesson lesson)
    {
      return new LessonResponseDto
      {
        Id = lesson.Id,
        Price = lesson.Price,
        Description = lesson.Description,
        ClassType = lesson.ClassType,
        MaxCapacity = lesson.MaxCapacity,
        CurrentEnrollments = lesson.CurrentEnrollments,
        CourtSlotId = lesson.CourtSlotId,
        Date = lesson.CourtSlot.Date,
        StartTime = lesson.CourtSlot.StartTime,
        EndTime = lesson.CourtSlot.EndTime,
        CourtId = lesson.CourtSlot.CourtId,
        CourtName = lesson.CourtSlot.Court?.Name ?? "Cancha no encontrada",
        TeacherId = lesson.TeacherId,
        TeacherName = lesson.Teacher?.Title ?? "Profesor no encontrado",
        TeacherTitle = lesson.Teacher?.Title,
        Enrollments = lesson.Enrollments?.Select(e => new LessonEnrollmentDto
        {
          Id = e.Id,
          EnrollmentDate = e.EnrollmentDate,
          PersonId = e.PersonId,
          StudentName = e.Person?.Category ?? "Estudiante",
          StudentCategory = e.Person?.Category ?? "",
          IsPaid = e.Payment != null,
          PaymentAmount = e.Payment?.Amount,
          PaymentDate = e.Payment?.CreatedAt // Usar CreatedAt como fecha de pago
        }).ToList() ?? new(),
        CreatedAt = lesson.CreatedAt,
        UpdatedAt = lesson.UpdatedAt
      };
    }

    private LessonListDto MapToLessonListDto(Lesson lesson, User? user)
    {
      var status = "Programada";
      if (lesson.HasEnded) status = "Finalizada";
      else if (lesson.HasStarted) status = "En Curso";

      return new LessonListDto
      {
        Id = lesson.Id,
        Price = lesson.Price,
        ClassType = lesson.ClassType,
        MaxCapacity = lesson.MaxCapacity,
        CurrentEnrollments = lesson.CurrentEnrollments,
        Date = lesson.CourtSlot.Date,
        StartTime = lesson.CourtSlot.StartTime,
        EndTime = lesson.CourtSlot.EndTime,
        CourtName = lesson.CourtSlot.Court?.Name ?? "Cancha no encontrada",
        TeacherName = user != null ? $"{user.Name} {user.Surname}" : "Profesor no encontrado",
        Status = status
      };
    }

    #endregion
  }
}