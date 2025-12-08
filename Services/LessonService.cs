using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Lesson;
using padelya_api.models;
using padelya_api.Models;
using padelya_api.Models.Lesson;
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
        // Usar FirstOrDefaultAsync en lugar de FindAsync para mejor compatibilidad con herencia
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == createDto.TeacherId);
        
        if (teacher == null)
        {
          // Log adicional para depuración
          var totalTeachers = await _context.Teachers.CountAsync();
          var allTeacherIds = await _context.Teachers.Select(t => t.Id).ToListAsync();
          
          Console.WriteLine($"⚠️ No se encontró profesor con ID {createDto.TeacherId}");
          Console.WriteLine($"   Total de profesores en DB: {totalTeachers}");
          Console.WriteLine($"   IDs de profesores disponibles: {string.Join(", ", allTeacherIds)}");
          
          return ResponseMessage<LessonResponseDto>.Error(
              $"El profesor con ID {createDto.TeacherId} no existe. " +
              $"Profesores disponibles: {string.Join(", ", allTeacherIds)}");
        }
        
        Console.WriteLine($"✅ Profesor encontrado: {teacher.Id}");

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
      // Si hay un estado manual, usarlo. Si no, calcularlo automáticamente
      string status;
      if (!string.IsNullOrEmpty(lesson.Status))
      {
        status = lesson.Status;
      }
      else
      {
        status = "Programada";
        if (lesson.HasEnded) status = "Finalizada";
        else if (lesson.HasStarted) status = "En Curso";
      }

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

    public async Task<ResponseMessage<bool>> UpdateLessonStatusAsync(int lessonId, string status)
    {
      try
      {
        var lesson = await _context.Lessons
            .Include(l => l.CourtSlot)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
          return ResponseMessage<bool>.NotFound("Clase no encontrada");
        }

        // Guardar el estado manual en el modelo Lesson
        lesson.Status = status;
        lesson.UpdatedAt = DateTime.UtcNow;

        // También actualizar CourtSlot.Status si es Cancelada
        if (status.ToLower() == "cancelada")
        {
          lesson.CourtSlot.Status = CourtSlotStatus.Cancelled;
          _context.CourtSlots.Update(lesson.CourtSlot);
        }
        else if (lesson.CourtSlot.Status == CourtSlotStatus.Cancelled && status.ToLower() != "cancelada")
        {
          // Si estaba cancelada y ahora no, reactivarla
          lesson.CourtSlot.Status = CourtSlotStatus.Active;
          _context.CourtSlots.Update(lesson.CourtSlot);
        }

        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();

        return ResponseMessage<bool>.SuccessResult(true, "Estado de la clase actualizado exitosamente");
      }
      catch (Exception ex)
      {
        return ResponseMessage<bool>.Error($"Error al actualizar el estado de la clase: {ex.Message}");
      }
    }

    #endregion

    #region Reports

    public async Task<ResponseMessage<LessonReportDto>> GetLessonReportAsync(DateTime startDate, DateTime endDate)
    {
      try
      {
        // Obtener todas las clases en el rango de fechas
        var lessons = await _context.Lessons
            .Include(l => l.Teacher)
            .Include(l => l.CourtSlot)
            .ThenInclude(cs => cs.Court)
            .Where(l => l.CourtSlot.Date >= startDate.Date && l.CourtSlot.Date <= endDate.Date)
            .ToListAsync();

        // Obtener inscripciones
        var lessonIds = lessons.Select(l => l.Id).ToList();
        var enrollments = await _context.LessonEnrollments
            .Include(e => e.Person)
            .Where(e => lessonIds.Contains(e.LessonId))
            .ToListAsync();

        // Obtener asistencias
        var attendances = await _context.LessonAttendances
            .Where(a => lessonIds.Contains(a.LessonId))
            .ToListAsync();

        // Obtener asignaciones de rutinas
        var routineAssignments = await _context.LessonRoutineAssignments
            .Include(ra => ra.Routine)
            .ThenInclude(r => r.Exercises)
            .Include(ra => ra.Routine.Creator)
            .Where(ra => lessonIds.Contains(ra.LessonId))
            .ToListAsync();

        // Calcular estadísticas generales
        var totalLessons = lessons.Count;
        var programmedLessons = lessons.Count(l => l.Status != null && l.Status.ToLower() == "programada");
        var completedLessons = lessons.Count(l => l.Status != null && l.Status.ToLower() == "finalizada");
        var cancelledLessons = lessons.Count(l => l.Status != null && l.Status.ToLower() == "cancelada");
        var totalEnrollments = enrollments.Count;
        var avgEnrollmentsPerLesson = totalLessons > 0 ? (decimal)totalEnrollments / totalLessons : 0;
        
        var totalAttendances = attendances.Count;
        var presentAttendances = attendances.Count(a => a.Status == AttendanceStatus.Present);
        var attendanceRate = totalAttendances > 0 ? (decimal)presentAttendances / totalAttendances * 100 : 0;

        var totalRoutinesAssigned = routineAssignments.Count;
        var uniqueExercises = routineAssignments
            .Where(ra => ra.Routine != null && ra.Routine.Exercises != null)
            .SelectMany(ra => ra.Routine.Exercises)
            .Select(e => e.Id)
            .Distinct()
            .Count();

        var activeTeachers = lessons.Select(l => l.TeacherId).Distinct().Count();
        var activeStudents = enrollments.Select(e => e.PersonId).Distinct().Count();
        
        var totalRevenue = lessons
            .Where(l => l.Status == null || l.Status.ToLower() != "cancelada")
            .Sum(l => l.Price);

        var statistics = new LessonStatisticsDto
        {
          TotalLessons = totalLessons,
          ProgrammedLessons = programmedLessons,
          CompletedLessons = completedLessons,
          CancelledLessons = cancelledLessons,
          TotalEnrollments = totalEnrollments,
          AverageEnrollmentsPerLesson = avgEnrollmentsPerLesson,
          AttendanceRate = attendanceRate,
          TotalRoutinesAssigned = totalRoutinesAssigned,
          TotalExercisesUsed = uniqueExercises,
          ActiveTeachers = activeTeachers,
          ActiveStudents = activeStudents,
          TotalRevenue = totalRevenue
        };

        // Clases por día
        var dailyLessons = lessons
            .GroupBy(l => l.CourtSlot.Date.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailyLessonDto
            {
              Date = g.Key.ToString("yyyy-MM-dd"),
              LessonCount = g.Count(),
              EnrollmentCount = enrollments.Count(e => g.Select(l => l.Id).Contains(e.LessonId)),
              AttendedCount = attendances.Count(a => g.Select(l => l.Id).Contains(a.LessonId) && a.Status == AttendanceStatus.Present)
            })
            .ToList();

        // Rendimiento de profesores
        var teacherPerformance = new List<TeacherPerformanceDto>();
        foreach (var teacherGroup in lessons.GroupBy(l => l.TeacherId))
        {
          var teacher = teacherGroup.First().Teacher;
          var teacherName = (teacher != null) ? teacher.Name + " " + teacher.Surname : "Desconocido";
          var teacherLessonIds = teacherGroup.Select(l => l.Id).ToList();
          var teacherAttendances = attendances.Where(a => teacherLessonIds.Contains(a.LessonId)).ToList();
          
          teacherPerformance.Add(new TeacherPerformanceDto
          {
            TeacherId = teacherGroup.Key,
            TeacherName = teacherName,
            LessonCount = teacherGroup.Count(),
            TotalStudents = enrollments.Count(e => teacherLessonIds.Contains(e.LessonId)),
            AverageAttendance = teacherAttendances.Count > 0
                ? (decimal)teacherAttendances.Count(a => a.Status == AttendanceStatus.Present) / teacherAttendances.Count * 100
                : 0,
            RoutinesCreated = _context.Routines.Count(r => r.CreatorId == teacherGroup.Key)
          });
        }
        teacherPerformance = teacherPerformance.OrderByDescending(t => t.LessonCount).ToList();

        // Distribución por tipo de clase
        var classTypeDistribution = lessons
            .GroupBy(l => l.ClassType ?? "Sin tipo")
            .Select(g => new ClassTypeDistributionDto
            {
              ClassTypeId = 0, // No hay ID ya que es un string
              ClassTypeName = g.Key,
              LessonCount = g.Count(),
              EnrollmentCount = enrollments.Count(e => g.Select(l => l.Id).Contains(e.LessonId)),
              Percentage = totalLessons > 0 ? (decimal)g.Count() / totalLessons * 100 : 0
            })
            .OrderByDescending(ct => ct.LessonCount)
            .ToList();

        // Distribución de asistencia
        var attendanceDistribution = new List<AttendanceDistributionDto>
        {
          new AttendanceDistributionDto
          {
            Status = "Presente",
            Count = attendances.Count(a => a.Status == AttendanceStatus.Present),
            Percentage = totalAttendances > 0 ? (decimal)attendances.Count(a => a.Status == AttendanceStatus.Present) / totalAttendances * 100 : 0
          },
          new AttendanceDistributionDto
          {
            Status = "Ausente",
            Count = attendances.Count(a => a.Status == AttendanceStatus.Absent),
            Percentage = totalAttendances > 0 ? (decimal)attendances.Count(a => a.Status == AttendanceStatus.Absent) / totalAttendances * 100 : 0
          },
          new AttendanceDistributionDto
          {
            Status = "Justificado",
            Count = attendances.Count(a => a.Status == AttendanceStatus.Justified),
            Percentage = totalAttendances > 0 ? (decimal)attendances.Count(a => a.Status == AttendanceStatus.Justified) / totalAttendances * 100 : 0
          }
        };

        // Top rutinas más asignadas
        var topRoutines = new List<TopRoutineDto>();
        var routineGroups = routineAssignments
            .Where(ra => ra.Routine != null)
            .GroupBy(ra => ra.RoutineId);
        
        foreach (var group in routineGroups)
        {
          var routine = group.First().Routine;
          if (routine != null)
          {
            var creatorName = "Desconocido";
            if (routine.Creator != null)
            {
              creatorName = routine.Creator.Name + " " + routine.Creator.Surname;
            }
            
            topRoutines.Add(new TopRoutineDto
            {
              RoutineId = routine.Id,
              Category = routine.Category,
              Description = routine.Description,
              AssignmentCount = group.Count(),
              CreatorName = creatorName
            });
          }
        }
        topRoutines = topRoutines.OrderByDescending(r => r.AssignmentCount).Take(10).ToList();

        // Top ejercicios más usados
        var allRoutines = routineAssignments
            .Where(ra => ra.Routine != null && ra.Routine.Exercises != null)
            .Select(ra => ra.Routine)
            .Distinct()
            .ToList();

        var exerciseUsage = new Dictionary<int, (string Name, string Category, int Count)>();
        foreach (var routine in allRoutines)
        {
          if (routine.Exercises != null)
          {
            foreach (var exercise in routine.Exercises)
            {
              if (exerciseUsage.ContainsKey(exercise.Id))
              {
                var current = exerciseUsage[exercise.Id];
                exerciseUsage[exercise.Id] = (current.Name, current.Category, current.Count + 1);
              }
              else
              {
                exerciseUsage[exercise.Id] = (exercise.Name, exercise.Category, 1);
              }
            }
          }
        }

        var topExercises = exerciseUsage
            .Select(kvp => new TopExerciseDto
            {
              ExerciseId = kvp.Key,
              Name = kvp.Value.Name,
              Category = kvp.Value.Category,
              UsageCount = kvp.Value.Count
            })
            .OrderByDescending(e => e.UsageCount)
            .Take(10)
            .ToList();

        // Top estudiantes por asistencia
        var studentAttendance = new List<StudentAttendanceDto>();
        var studentGroups = enrollments.GroupBy(e => e.PersonId);
        
        foreach (var group in studentGroups)
        {
          var person = group.First().Person;
          var studentName = person?.Name + " " + person?.Surname ?? "Desconocido";
          var totalClasses = group.Count();
          var attendedClasses = attendances.Count(a => a.PersonId == group.Key && a.Status == AttendanceStatus.Present);
          
          studentAttendance.Add(new StudentAttendanceDto
          {
            StudentId = group.Key,
            StudentName = studentName,
            TotalClasses = totalClasses,
            AttendedClasses = attendedClasses,
            AttendanceRate = totalClasses > 0 ? (decimal)attendedClasses / totalClasses * 100 : 0
          });
        }
        studentAttendance = studentAttendance
            .OrderByDescending(s => s.AttendanceRate)
            .ThenByDescending(s => s.TotalClasses)
            .Take(10)
            .ToList();

        var report = new LessonReportDto
        {
          Statistics = statistics,
          DailyLessons = dailyLessons,
          TeacherPerformance = teacherPerformance,
          ClassTypeDistribution = classTypeDistribution,
          AttendanceDistribution = attendanceDistribution,
          TopRoutines = topRoutines,
          TopExercises = topExercises,
          TopStudents = studentAttendance
        };

        return ResponseMessage<LessonReportDto>.SuccessResult(report);
      }
      catch (Exception ex)
      {
        return ResponseMessage<LessonReportDto>.Error($"Error al generar reporte: {ex.Message}");
      }
    }

    #endregion
  }
}