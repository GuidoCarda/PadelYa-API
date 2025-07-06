using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Booking;
using padelya_api.DTOs.Payment;
using padelya_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace padelya_api.Services
{
    public class BookingService : IBookingService
    {
        private readonly PadelYaDbContext _context;
        private readonly ICourtSlotService _courtSlotService;

        public BookingService(PadelYaDbContext context, ICourtSlotService courtSlotService)
        {
            _context = context;
            _courtSlotService = courtSlotService;
        }

        public async Task<IEnumerable<BookingDto>> GetAllAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.CourtSlot)
                .Include(b => b.Person)
                .ToListAsync();


            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CourtSlotId = b.CourtSlotId,
                PersonId = b.PersonId,
            });
        }

        public async Task<BookingDto> GetByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(bk => bk.CourtSlot)
                .Include(bk => bk.Person)
                .FirstOrDefaultAsync(bk => bk.Id == id);

            if (booking == null) return null;

            return new BookingDto
            {
                Id = booking.Id,
                CourtSlotId = booking.CourtSlotId,
                PersonId = booking.PersonId,
            };
        }

        public async Task<BookingDto> CreateAsync(BookingCreateDto dto)
        {
            var slot = await _courtSlotService.CreateSlotIfAvailableAsync(dto.CourtId, dto.Date, dto.StartTime, dto.EndTime);

            // Ahora crea el Booking asociado
            var booking = new Booking
            {
                CourtSlotId = slot.Id,
                PersonId = dto.PersonId,
                // Otros campos
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new BookingDto
            {
                Id = booking.Id,
                CourtSlotId = booking.CourtSlotId,
                PersonId = booking.PersonId,
                // Otros campos
            };
        }

        public async Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return null;

            // Actualiza solo los campos permitidos
            booking.CourtSlotId = dto.CourtSlotId;
            booking.PersonId = dto.PersonId;
            // Otros campos

            await _context.SaveChangesAsync();

            return new BookingDto
            {
                Id = booking.Id,
                CourtSlotId = booking.CourtSlotId,
                PersonId = booking.PersonId,
                // Otros campos
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookingResponseDto> CreateWithPaymentAsync(BookingCreateDto dto)
        {
            Console.WriteLine($"Iniciando creación de reserva: CourtId={dto.CourtId}, Date={dto.Date}, PersonId={dto.PersonId}");

            var slot = await _courtSlotService.CreateSlotIfAvailableAsync(dto.CourtId, dto.Date, dto.StartTime, dto.EndTime);
            Console.WriteLine($"Slot creado: Id={slot.Id}");

            var court = await _context.Courts.FirstOrDefaultAsync(c => c.Id == dto.CourtId);
            if (court == null)
                throw new Exception("Court not found.");

            decimal amount = 0;
            string paymentType = dto.PaymentType?.ToLower();
            if (paymentType == "deposit")
                amount = court.BookingPrice * 0.5m;
            else if (paymentType == "total")
                amount = court.BookingPrice;
            else
                throw new Exception("Tipo de pago inválido. Use 'deposit' o 'total'.");

            Console.WriteLine($"Monto calculado: {amount}");

            // Crear el Booking primero
            var booking = new Booking
            {
                CourtSlotId = slot.Id,
                PersonId = dto.PersonId
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Booking creado: Id={booking.Id}");

            // Crear el Payment y asociarlo al Booking
            var payment = new Payment
            {
                Amount = amount,
                PaymentMethod = "Simulado",
                PaymentStatus = "Aprobado",
                CreatedAt = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString(),
                PersonId = dto.PersonId,
                BookingId = booking.Id  // Asociar el pago a la reserva
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Payment creado: Id={payment.Id}");

            // Mapear a DTOs
            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                CourtSlotId = booking.CourtSlotId,
                PersonId = booking.PersonId,
                Payments = new List<PaymentDto> { new PaymentDto
                {
                    Id = payment.Id,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentStatus = payment.PaymentStatus,
                    CreatedAt = payment.CreatedAt,
                    TransactionId = payment.TransactionId,
                    PersonId = payment.PersonId
                }}
            };

            var paymentDto = new PaymentDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                CreatedAt = payment.CreatedAt,
                TransactionId = payment.TransactionId,
                PersonId = payment.PersonId
            };

            Console.WriteLine($"BookingDto: Id={bookingDto.Id}, CourtSlotId={bookingDto.CourtSlotId}, PersonId={bookingDto.PersonId}");
            Console.WriteLine($"PaymentDto: Id={paymentDto.Id}, Amount={paymentDto.Amount}");

            var response = new BookingResponseDto
            {
                Booking = bookingDto,
                Payment = paymentDto
            };

            Console.WriteLine($"Response creado: BookingId={response.Booking.Id}, PaymentId={response.Payment.Id}");

            return response;
        }
    }
}