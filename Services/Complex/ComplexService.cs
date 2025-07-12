using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs;
using padelya_api.DTOs.Court;
using padelya_api.DTOs.User;
using padelya_api.Models;

namespace padelya_api.Services
{
  public class ComplexService(PadelYaDbContext context) : IComplexService
  {
    private readonly PadelYaDbContext _context = context;

    public async Task<Complex> GetComplexAsync()
    {
      var complex = await _context.Complex.FirstAsync();
      return complex;
    }

    public async Task<List<CourtListDto>> GetCourtsAsync()
    {
      var courts = await _context.Courts
          .Where(c => c.CourtStatus != CourtStatus.Deleted)
          .ToListAsync();

      var activeSlots = await _context.CourtSlots
        .Where(cs => cs.Status == CourtSlotStatus.Active
          && cs.Date >= DateTime.Now.Date)
        .ToListAsync();

      var result = courts.Select(court =>
      {
        var slotsCount = activeSlots.Count(cs => cs.CourtId == court.Id);
        Console.WriteLine(slotsCount);
        return new CourtListDto
        {
          Id = court.Id,
          Name = court.Name,
          CourtStatus = court.CourtStatus,
          BookingPrice = court.BookingPrice,
          OpeningTime = court.OpeningTime,
          ClosingTime = court.ClosingTime,
          ComplexId = court.ComplexId,
          Type = court.Type,
          ActiveSlotsCount = slotsCount
        };
      }).ToList();


      return result;
    }

    public async Task<Court?> GetCourtByIdAsync(int id)
    {
      var court = await _context.Courts
          .Where(c => c.CourtStatus != CourtStatus.Deleted)
          .FirstOrDefaultAsync(c => c.Id == id);
      return court;
    }


    public async Task<Court?> CreateCourtAsync(CreateCourtDto courtDto)
    {
      var court = new Court
      {
        Name = courtDto.Name,
        BookingPrice = courtDto.BookingPrice,
        CourtStatus = courtDto.CourtStatus,
        ComplexId = courtDto.ComplexId,
        OpeningTime = courtDto.OpeningTime,
        ClosingTime = courtDto.ClosingTime,
        Type = courtDto.Type
      };

      _context.Courts.Add(court);
      await _context.SaveChangesAsync();

      return await _context.Courts
          .FirstOrDefaultAsync(c => c.Id == court.Id);
    }

    public async Task<Court?> UpdateCourtAsync(int id, [FromBody] UpdateCourtDto updateDto)
    {

      var court = await _context.Courts
          .FirstOrDefaultAsync(c => c.Id == id);

      if (court == null)
      {
        return null;
      }

      if (updateDto.CourtStatus.HasValue &&
        updateDto.CourtStatus.Value == CourtStatus.Maintenance)
      {
        var activeSlots = await _context.CourtSlots
         .CountAsync(cs => cs.Status == CourtSlotStatus.Active && cs.CourtId == id && cs.Date >= DateTime.Now.Date);

        if (activeSlots > 0)
        {
          throw new InvalidOperationException(
               $"No se puede cambiar la cancha a mantenimiento. Tiene {activeSlots} slots activos.");
        }
      }

      if (updateDto.BookingPrice != null)
      {
        court.BookingPrice = updateDto.BookingPrice.Value;
      }

      if (!string.IsNullOrWhiteSpace(updateDto.Name))
      {
        court.Name = updateDto.Name;
      }

      if (updateDto.CourtStatus.HasValue)
      {
        court.CourtStatus = updateDto.CourtStatus.Value;
      }

      if (updateDto.OpeningTime.HasValue)
      {
        court.OpeningTime = updateDto.OpeningTime.Value;
      }

      if (updateDto.ClosingTime.HasValue)
      {
        court.ClosingTime = updateDto.ClosingTime.Value;
      }

      if (!string.IsNullOrWhiteSpace(updateDto.Type))
      {
        court.Type = updateDto.Type;
      }

      await _context.SaveChangesAsync();
      return court;
    }

    public async Task<bool> DeleteCourtAsync(int id)
    {
      var court = await _context.Courts.FirstOrDefaultAsync(c => c.Id == id);

      if (court == null)
      {
        return false; // court not found
      }

      var activeCourtSlots = await _context.CourtSlots
        .Where(cs => cs.CourtId == id
          && cs.Status == CourtSlotStatus.Active
          && cs.Date >= DateTime.Today)
        .ToListAsync();

      if (activeCourtSlots.Any())
      {
        return false; // cannot delete court with active future bookings
      }

      court.CourtStatus = CourtStatus.Deleted;
      await _context.SaveChangesAsync();
      return true;
    }
  }
}