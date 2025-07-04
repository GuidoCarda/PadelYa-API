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

        public async Task<List<Court>> GetCourtsAsync()
        {
            var courts = await _context.Courts
                .Include(c => c.Availability)
                .Where(c => c.CourtStatus != CourtStatus.Deleted)
                .ToListAsync();

            return courts;
        }

        public async Task<Court?> GetCourtByIdAsync(int id)
        {
            var court = await _context.Courts
                .Include(c => c.Availability)
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
            };

            _context.Courts.Add(court);
            await _context.SaveChangesAsync();


            var weekdays = Enum.GetValues<Weekday>();
            var defaultAvailability = new List<CourtAvailability>();
            foreach (var weekday in weekdays)
            {
                var availability = new CourtAvailability
                {
                    Weekday = weekday,
                    StartTime = new TimeOnly(8, 0), // 8:00 AM
                    EndTime = new TimeOnly(22, 0), // 10:00 PM
                    CourtId = court.Id
                };

                defaultAvailability.Add(availability);
            }

            _context.CourtAvailabilities.AddRange(defaultAvailability);
            await _context.SaveChangesAsync();

            return await _context.Courts
                .Include(c => c.Availability)
                .FirstOrDefaultAsync(c => c.Id == court.Id);
        }

        public async Task<Court?> UpdateCourtAsync(int id, [FromBody] UpdateCourtDto updateDto)
        {

            var court = await _context.Courts
                .Include(c => c.Availability)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (court == null)
            {
                return null;
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
            court.CourtStatus = CourtStatus.Deleted;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}