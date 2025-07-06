using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Booking;
using padelya_api.Services;

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
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
                return NotFound();
            return Ok(booking);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                Console.WriteLine($"Controlador: Recibiendo request para crear reserva");
                var result = await _bookingService.CreateWithPaymentAsync(dto);

                Console.WriteLine($"Controlador: Resultado recibido - BookingId: {result.Booking.Id}, PaymentId: {result.Payment.Id}");
                return CreatedAtAction(nameof(GetById), new { id = result.Booking.Id }, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Controlador: Error - {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
        // {
        //     var created = await _bookingService.CreateAsync(dto);
        //     return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        // }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _bookingService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bookingService.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
