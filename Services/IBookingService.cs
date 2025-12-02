using padelya_api.DTOs.Booking;
using padelya_api.DTOs.Payment;
using padelya_api.DTOs.Complex;

namespace padelya_api.Services
{
  public interface IBookingService
  {
    Task<IEnumerable<BookingDto>> GetAllAsync(string? email = null, string? status = null, string? startDate = null, string? endDate = null);
    Task<BookingDto?> GetByIdAsync(int id);
    Task<BookingDto> CreateAsync(BookingCreateDto dto);

    Task<ReservationInitPointDto> CreateWithPaymentAsync(BookingReserveWithPaymentDto dto);

    Task<BookingResponseDto> CreateAdminBookingAsync(BookingCreateDto dto);

    Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto);
    Task<bool> DeleteAsync(int id, string? cancelledBy);

    // Cancel pending of payment expired booking by id 
    Task<bool> CancelExpiredAsync(int id);



    Task<IEnumerable<CourtAvailabilityDto>> GetDailyAvailabilityAsync(DateTime date);

    Task<bool> CancelAsync(int id, CancelBookingDto dto);
    Task<BookingDto?> RegisterPaymentAsync(int id, RegisterPaymentDto paymentDto);

    // List bookings for the logged-in user (maps userId -> personId internally)
    Task<List<BookingDto>> GetUserBookingsAsync(int userId, string? status = null);

    // List bookings by domain subject (Person)
    Task<List<BookingDto>> GetBookingsByPersonIdAsync(int personId, string? status = null);

    // Get booking report with statistics and analytics
    Task<BookingReportDto> GetBookingReportAsync(DateTime startDate, DateTime endDate);
  }
}