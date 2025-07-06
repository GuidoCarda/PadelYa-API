using padelya_api.Models;

namespace padelya_api.Services
{
    public interface ICourtSlotService
    {
        Task<bool> IsSlotAvailableAsync(int courtId, DateTime date, TimeOnly start, TimeOnly end);

        Task<CourtSlot> CreateSlotIfAvailableAsync(int courtId, DateTime date, TimeOnly start, TimeOnly end);

        Task<IEnumerable<(TimeOnly Start, TimeOnly End)>> GetAvailableSlotsAsync(int courtId, DateTime date);

        Task<IEnumerable<CourtSlot>> GetOccupiedSlotsAsync(int courtId, DateTime date);

        Task<IEnumerable<CourtSlot>> GetSlotsByDateRangeAsync(int courtId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<(DateTime Date, TimeOnly Start, TimeOnly End)>> GetAvailableSlotsByDateRangeAsync(
            int courtId, DateTime startDate, DateTime endDate);
    }
}