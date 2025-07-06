using padelya_api.DTOs.Booking;
using padelya_api.DTOs.Payment;

namespace padelya_api.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllAsync();
        Task<BookingDto> GetByIdAsync(int id);
        Task<BookingDto> CreateAsync(BookingCreateDto dto);

        Task<BookingResponseDto> CreateWithPaymentAsync(BookingCreateDto dto);

        Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}