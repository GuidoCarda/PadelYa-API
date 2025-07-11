using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Booking;
using padelya_api.Services;
using padelya_api.Shared;
using padelya_api.DTOs.Complex;

namespace padelya_api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BookingController : ControllerBase
  {
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
      _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? email, [FromQuery] string? status)
    {
      try
      {
        var bookings = await _bookingService.GetAllAsync(email, status);
        return Ok(ResponseMessage<IEnumerable<BookingDto>>.SuccessResult(bookings, "Bookings retrieved successfully"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<IEnumerable<BookingDto>>.Error($"Failed to retrieve bookings: {ex.Message}"));
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      try
      {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking == null)
          return NotFound(ResponseMessage<BookingDto>.NotFound($"Booking with ID {id} not found"));

        return Ok(ResponseMessage<BookingDto>.SuccessResult(booking, "Booking retrieved successfully"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<BookingDto>.Error($"Failed to retrieve booking: {ex.Message}"));
      }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
    {
      if (!ModelState.IsValid)
      {
        var validationErrors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return BadRequest(ResponseMessage<BookingResponseDto>.ValidationError("Validation failed", validationErrors));
      }

      try
      {
        Console.WriteLine($"Controlador: Recibiendo request para crear reserva");
        var result = await _bookingService.CreateWithPaymentAsync(dto);

        Console.WriteLine($"Controlador: Resultado recibido - BookingId: {result.Booking.Id}, PaymentId: {result.Payment.Id}");
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Booking.Id },
            ResponseMessage<BookingResponseDto>.SuccessResult(result, "Booking created successfully")
        );
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Controlador: Error - {ex.Message}");
        return BadRequest(ResponseMessage<BookingResponseDto>.Error($"Failed to create booking: {ex.Message}"));
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BookingUpdateDto dto)
    {
      if (!ModelState.IsValid)
      {
        var validationErrors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return BadRequest(ResponseMessage<BookingDto>.ValidationError("Validation failed", validationErrors));
      }

      try
      {
        var updated = await _bookingService.UpdateAsync(id, dto);
        if (updated == null)
          return NotFound(ResponseMessage<BookingDto>.NotFound($"Booking with ID {id} not found"));

        return Ok(ResponseMessage<BookingDto>.SuccessResult(updated, "Booking updated successfully"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<BookingDto>.Error($"Failed to update booking: {ex.Message}"));
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, string? cancelledBy = "admin")
    {
      try
      {
        var deleted = await _bookingService.DeleteAsync(id, cancelledBy);
        if (!deleted)
          return NotFound(ResponseMessage.NotFound($"Booking with ID {id} not found"));

        return Ok(ResponseMessage.SuccessMessage("Booking deleted successfully"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage.Error($"Failed to delete booking: {ex.Message}"));
      }
    }

    [HttpGet("availability")]
    public async Task<IActionResult> GetDailyAvailability([FromQuery] string date)
    {
      if (!DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate))
      {
        return BadRequest(ResponseMessage.Error("El formato de fecha debe ser YYYY-MM-DD"));
      }
      var availability = await _bookingService.GetDailyAvailabilityAsync(parsedDate);
      return Ok(ResponseMessage<IEnumerable<CourtAvailabilityDto>>.SuccessResult(availability, "Disponibilidad obtenida correctamente"));
    }
  }
}
