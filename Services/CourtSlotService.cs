using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.Models;

namespace padelya_api.Services
{
  public class CourtSlotService : ICourtSlotService
  {
    private readonly PadelYaDbContext _context;
    public CourtSlotService(PadelYaDbContext context)
    {
      _context = context;
    }

    public async Task<bool> IsSlotAvailableAsync(int courtId, DateTime date, TimeOnly start, TimeOnly end)
    {
      var slot = await _context.CourtSlots
          .Include(cs => cs.Booking)
          .Include(cs => cs.Lesson)
          .Include(cs => cs.TournamentMatch)
          .FirstOrDefaultAsync(cs =>
              cs.CourtId == courtId &&
              cs.Date == date &&
              cs.StartTime == start &&
              cs.EndTime == end);
      return slot == null;
    }

    public async Task<CourtSlot> CreateSlotIfAvailableAsync(int courtId, DateTime date, TimeOnly start, TimeOnly end)
    {
      // Valida horario de apertura/cierre
      var court = await _context.Courts
        .FirstOrDefaultAsync(c => c.Id == courtId);

      if (court == null)
        throw new Exception("Court not found.");

      Console.WriteLine(start);
      Console.WriteLine(court.OpeningTime);
      Console.WriteLine(end);
      Console.WriteLine(court.ClosingTime);

      if (start < court.OpeningTime || end > court.ClosingTime)
        throw new Exception("El turno está fuera del horario permitido para la cancha.");

      // Verifica si ya existe un CourtSlot ACTIVO para ese turno
      var slot = await _context.CourtSlots
          .Include(cs => cs.Booking)
          .Include(cs => cs.Lesson)
          .Include(cs => cs.TournamentMatch)
          .FirstOrDefaultAsync(cs =>
              cs.CourtId == courtId &&
              cs.Date == date &&
              cs.StartTime == start &&
              cs.EndTime == end &&
              (cs.Status == CourtSlotStatus.Active || cs.Status == CourtSlotStatus.Pending));

      if (slot != null)
        throw new Exception("Ese turno ya está ocupado.");

      // Si no existe un slot activo, lo crea
      slot = new CourtSlot
      {
        CourtId = courtId,
        Date = date,
        StartTime = start,
        EndTime = end
      };
      _context.CourtSlots.Add(slot);
      await _context.SaveChangesAsync();
      return slot;
    }

    public async Task<IEnumerable<(TimeOnly Start, TimeOnly End)>> GetAvailableSlotsAsync(int courtId, DateTime date)
    {
      var court = await _context.Courts
          .FirstOrDefaultAsync(c => c.Id == courtId);

      if (court == null) throw new Exception("Court not found.");

      var opening = court.OpeningTime;
      var closing = court.ClosingTime;
      var slotDuration = SlotConfig.SlotDuration;

      var occupiedSlots = await _context.CourtSlots
          .Where(cs => cs.CourtId == courtId && cs.Date == date)
          .Select(cs => new { cs.StartTime, cs.EndTime })
          .ToListAsync();

      var availableSlots = new List<(TimeOnly Start, TimeOnly End)>();
      var currentStart = opening;
      while (currentStart.Add(slotDuration) <= closing)
      {
        var currentEnd = currentStart.Add(slotDuration);

        bool isOccupied = occupiedSlots.Any(os =>
            currentStart < os.EndTime && currentEnd > os.StartTime
        );

        if (!isOccupied)
          availableSlots.Add((currentStart, currentEnd));

        currentStart = currentStart.Add(slotDuration);
      }

      return availableSlots;
    }

    public async Task<IEnumerable<CourtSlot>> GetOccupiedSlotsAsync(int courtId, DateTime date)
    {
      var court = await _context.Courts.FirstOrDefaultAsync(c => c.Id == courtId);

      if (court == null) throw new Exception("Court not found.");

      var occupiedSlots = await _context.CourtSlots
          .Where(cs => cs.Date == date)
          .ToListAsync();

      return occupiedSlots;
    }

    public async Task<IEnumerable<CourtSlot>> GetSlotsByDateRangeAsync(int courtId, DateTime startDate, DateTime endDate)
    {
      return await _context.CourtSlots
          .Where(cs => cs.CourtId == courtId && cs.Date >= startDate && cs.Date <= endDate)
          .Include(cs => cs.Booking)
          .Include(cs => cs.Lesson)
          .Include(cs => cs.TournamentMatch)
          .ToListAsync();
    }

    public async Task<IEnumerable<(DateTime Date, TimeOnly Start, TimeOnly End)>> GetAvailableSlotsByDateRangeAsync(
        int courtId, DateTime startDate, DateTime endDate)
    {
      var court = await _context.Courts
          .FirstOrDefaultAsync(c => c.Id == courtId);

      if (court == null)
        throw new Exception("Court not found.");

      var slotDuration = SlotConfig.SlotDuration;
      var availableSlots = new List<(DateTime Date, TimeOnly Start, TimeOnly End)>();

      for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
      {
        var opening = court.OpeningTime;
        var closing = court.ClosingTime;

        var occupiedSlots = await _context.CourtSlots
            .Where(cs => cs.CourtId == courtId && cs.Date == date)
            .Select(cs => new { cs.StartTime, cs.EndTime })
            .ToListAsync();

        var currentStart = opening;
        while (currentStart.Add(slotDuration) <= closing)
        {
          var currentEnd = currentStart.Add(slotDuration);

          bool isOccupied = occupiedSlots.Any(os =>
              currentStart < os.EndTime && currentEnd > os.StartTime
          );

          if (!isOccupied)
            availableSlots.Add((date, currentStart, currentEnd));

          currentStart = currentStart.Add(slotDuration);
        }
      }

      return availableSlots;
    }
  }

  public static class SlotConfig
  {
    public static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(90);
  }
}