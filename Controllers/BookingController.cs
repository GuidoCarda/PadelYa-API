using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Booking;
using padelya_api.Services;
using padelya_api.Shared;
using padelya_api.DTOs.Complex;
using padelya_api.DTOs.Payment;

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
        return Ok(ResponseMessage<IEnumerable<BookingDto>>.SuccessResult(bookings, "Reservas obtenidas correctamente"));
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

        return Ok(ResponseMessage<BookingDto>.SuccessResult(booking, "Reserva obtenida correctamente"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<BookingDto>.Error($"Failed to retrieve booking: {ex.Message}"));
      }
    }

    // [HttpPost("admin")]
    // public async Task<IActionResult> CreateWithPayment([FromBody] BookingCreateDto dto)
    // {
    //   if (!ModelState.IsValid)
    //   {
    //     var validationErrors = ModelState
    //         .Where(x => x.Value.Errors.Count > 0)
    //         .ToDictionary(
    //             kvp => kvp.Key,
    //             kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
    //         );

    //     return BadRequest(ResponseMessage<BookingResponseDto>.ValidationError("Validation failed", validationErrors));
    //   }

    //   try
    //   {
    //     Console.WriteLine($"Controlador: Recibiendo request para crear reserva");
    //     var result = await _bookingService.CreateWithPaymentAsync(dto);

    //     Console.WriteLine($"Controlador: Resultado recibido - BookingId: {result.Booking.Id}, PaymentId: {result.Payment.Id}");
    //     return CreatedAtAction(
    //         nameof(GetById),
    //         new { id = result.Booking.Id },
    //         ResponseMessage<BookingResponseDto>.SuccessResult(result, "Reserva creada correctamente")
    //     );
    //   }
    //   catch (Exception ex)
    //   {
    //     Console.WriteLine($"Controlador: Error - {ex.Message}");
    //     return BadRequest(ResponseMessage<BookingResponseDto>.Error($"Error al crear reserva: {ex.Message}"));
    //   }
    // }


    [HttpPost]
    public async Task<IActionResult> ReserveWithPayment([FromBody] BookingReserveWithPaymentDto dto)
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
        Console.WriteLine($"Controlador: Recibiendo request para reservar turno con pago");
        var result = await _bookingService.CreateWithPaymentAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.init_point },
            ResponseMessage<ReservationInitPointDto>.SuccessResult(result, "URL de pago creada correctamente")
        );
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Controlador: Error - {ex.Message}");
        return BadRequest(ResponseMessage<BookingResponseDto>.Error($"Error al crear reserva: {ex.Message}"));
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

        return BadRequest(ResponseMessage<BookingDto>.ValidationError("Error al actualizar reserva con id {id}", validationErrors));
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
        return BadRequest(ResponseMessage<BookingDto>.Error($"Error al actualizar reserva con id {id}: {ex.Message}"));
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
        return BadRequest(ResponseMessage.Error($"Error al eliminar reserva con id {id}: {ex.Message}"));
      }
    }


    [HttpPost("{id}/payment")]
    public async Task<IActionResult> RegisterPayment(int id, [FromBody] RegisterPaymentDto dto)
    {
      try
      {
        var result = await _bookingService.RegisterPaymentAsync(id, dto);
        if (result == null)
          return NotFound(ResponseMessage<BookingDto>.NotFound($"Reserva con id {id} no encontrada"));

        return Ok(ResponseMessage<BookingDto>.SuccessResult(result, "Booking updated successfully"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage.Error($"Error al registrar pago en reserva con id {id}: {ex.Message}"));
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



    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserBookings(int userId, [FromQuery] string? status)
    {
      try
      {
        var bookings = await _bookingService.GetUserBookingsAsync(userId, status);
        return Ok(ResponseMessage<IEnumerable<BookingDto>>.SuccessResult(bookings, "Reservas del usuario obtenidas correctamente"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<IEnumerable<BookingDto>>.Error($"Failed to retrieve user bookings: {ex.Message}"));
      }
    }

    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetPersonBookings(int personId, [FromQuery] string? status)
    {
      try
      {
        var bookings = await _bookingService.GetBookingsByPersonIdAsync(personId, status);
        return Ok(ResponseMessage<IEnumerable<BookingDto>>.SuccessResult(bookings, "Reservas por persona obtenidas correctamente"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<IEnumerable<BookingDto>>.Error($"Failed to retrieve person bookings: {ex.Message}"));
      }
    }
  }
}
