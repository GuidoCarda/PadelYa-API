using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Booking;
using padelya_api.DTOs.Payment;
using padelya_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using padelya_api.DTOs.Complex;
using padelya_api.Constants;

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

    public async Task<IEnumerable<BookingDto>> GetAllAsync(string? email = null, string? status = null)
    {

      var query = _context.Bookings
          .Include(b => b.CourtSlot)
          .Include(b => b.CourtSlot.Court)
          .Include(b => b.Person)
          .Include(b => b.Payments)
          .AsQueryable();

      // Aplicar filtros
      if (!string.IsNullOrEmpty(email))
      {
        var userEmails = _context.Users
            .Where(u => u.Email.Contains(email))
            .Select(u => u.PersonId)
            .ToList();

        query = query.Where(b => userEmails.Contains(b.PersonId));
      }

      if (!string.IsNullOrEmpty(status))
      {
        if (Enum.TryParse<BookingStatus>(status, true, out var statusEnum))
        {
          query = query.Where(b => b.Status == statusEnum);
        }
      }

      var bookings = await query.ToListAsync();

      return bookings.Select(b => new BookingDto
      {
        Id = b.Id,
        CourtSlotId = b.CourtSlotId,
        PersonId = b.PersonId,


        Status = b.Status,
        DisplayStatus = b.DisplayStatus,

        // Información del slot/cancha
        Date = b.CourtSlot.Date,
        StartTime = b.CourtSlot.StartTime,
        EndTime = b.CourtSlot.EndTime,
        CourtId = b.CourtSlot.Court.Id,
        CourtName = b.CourtSlot.Court.Name,
        CourtType = b.CourtSlot.Court.Type,

        // Información del usuario (obtener desde la tabla User)
        UserName = _context.Users.FirstOrDefault(u => u.PersonId == b.PersonId)?.Name ?? "",
        UserSurname = _context.Users.FirstOrDefault(u => u.PersonId == b.PersonId)?.Surname ?? "",
        UserEmail = _context.Users.FirstOrDefault(u => u.PersonId == b.PersonId)?.Email ?? "",

        // Información de pagos
        Payments = b.Payments.Select(p => new PaymentDto
        {
          Id = p.Id,
          Amount = p.Amount,
          PaymentMethod = p.PaymentMethod,
          PaymentStatus = p.PaymentStatus,
          CreatedAt = p.CreatedAt,
          TransactionId = p.TransactionId,
          PersonId = p.PersonId
        }).ToList(),
        TotalPaid = b.Payments.Sum(p => p.Amount),
        TotalAmount = b.CourtSlot.Court.BookingPrice
      });
    }

    public async Task<BookingDto> GetByIdAsync(int id)
    {
      var booking = await _context.Bookings
          .Include(bk => bk.CourtSlot)
          .Include(bk => bk.Payments)
          .Include(bk => bk.Person)
          .FirstOrDefaultAsync(bk => bk.Id == id);

      if (booking == null) return null;

      return new BookingDto
      {
        Id = booking.Id,
        CourtSlotId = booking.CourtSlotId,
        PersonId = booking.PersonId,
        Status = booking.Status
      };
    }

    public async Task<BookingDto> CreateAsync(BookingCreateDto dto)
    {
      var endTime = dto.StartTime.AddMinutes(90);
      var slot = await _courtSlotService.CreateSlotIfAvailableAsync(dto.CourtId, dto.Date, dto.StartTime, endTime);

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

    public async Task<bool> DeleteAsync(int id, string? cancelledBy)
    {
      var booking = await _context.Bookings
        .Include(b => b.CourtSlot)
        .FirstOrDefaultAsync(b => b.Id == id);

      if (booking == null) return false;

      booking.Status = cancelledBy == "admin"
        ? BookingStatus.CancelledByAdmin
        : BookingStatus.CancelledByClient;

      booking.CancelledBy = "admin";
      booking.CancelledAt = DateTime.Now;

      booking.CourtSlot.Status = CourtSlotStatus.Cancelled;

      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<BookingResponseDto> CreateWithPaymentAsync(BookingCreateDto dto)
    {
      Console.WriteLine($"Iniciando creación de reserva: CourtId={dto.CourtId}, Date={dto.Date}, PersonId={dto.PersonId}");

      var endTime = dto.StartTime.AddMinutes(90);
      var slot = await _courtSlotService.CreateSlotIfAvailableAsync(dto.CourtId, dto.Date, dto.StartTime, endTime);
      Console.WriteLine($"Slot creado: Id={slot.Id}");

      var court = await _context.Courts.FirstOrDefaultAsync(c => c.Id == dto.CourtId);
      if (court == null)
        throw new Exception("Court not found.");

      decimal amount = 0;
      PaymentType paymentType = dto.PaymentType;
      if (paymentType == PaymentType.Deposit)
        amount = court.BookingPrice * 0.5m;
      else if (paymentType == PaymentType.Deposit)
        amount = court.BookingPrice;
      else
        throw new Exception("Tipo de pago inválido. Use 'deposit' o 'total'.");

      Console.WriteLine($"Monto calculado: {amount}");

      // Crear el Booking primero
      var booking = new Booking
      {
        CourtSlotId = slot.Id,
        PersonId = dto.PersonId,
        Status = paymentType == PaymentType.Deposit ? BookingStatus.ReservedDeposit : BookingStatus.ReservedPaid
      };

      _context.Bookings.Add(booking);
      await _context.SaveChangesAsync();
      Console.WriteLine($"Booking creado: Id={booking.Id}");

      // Crear el Payment y asociarlo al Booking
      var payment = new Payment
      {
        Amount = amount,
        PaymentMethod = "Simulado",
        PaymentStatus = PaymentStatus.Approved,
        CreatedAt = DateTime.UtcNow,
        TransactionId = Guid.NewGuid().ToString(),
        PersonId = dto.PersonId,
        BookingId = booking.Id,  // Asociar el pago a la reserva
        PaymentType = paymentType // <--- obligatorio
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

    public async Task<IEnumerable<CourtAvailabilityDto>> GetDailyAvailabilityAsync(DateTime date)
    {
      var courts = await _context.Courts.ToListAsync();
      var occupiedSlots = await _context.CourtSlots
          .Where(cs => cs.Date.Date == date.Date && cs.Status == CourtSlotStatus.Active)
          .Select(cs => new { cs.CourtId, cs.StartTime, cs.EndTime })
          .ToListAsync();

      var result = new List<CourtAvailabilityDto>();
      foreach (var court in courts)
      {
        var slots = GenerateSlotsForCourt(court, date);
        var slotDtos = new List<SlotAvailabilityDto>();
        foreach (var slot in slots)
        {
          bool isOccupied = occupiedSlots.Any(os => os.CourtId == court.Id && os.StartTime == slot.start && os.EndTime == slot.end);
          slotDtos.Add(new SlotAvailabilityDto
          {
            Start = slot.start.ToString("HH:mm"),
            End = slot.end.ToString("HH:mm"),
            Status = isOccupied ? "booked" : "available"
          });
        }
        result.Add(new CourtAvailabilityDto
        {
          CourtId = court.Id,
          CourtName = court.Name,
          Type = court.Type,
          BookingPrice = court.BookingPrice,
          Slots = slotDtos
        });
      }
      return result;
    }

    // Helper para generar slots posibles de 90 minutos
    private List<(TimeOnly start, TimeOnly end)> GenerateSlotsForCourt(Models.Court court, DateTime date)
    {
      var slots = new List<(TimeOnly, TimeOnly)>();

      // Validar que la cancha tenga horarios válidos
      if (court.OpeningTime == TimeOnly.MinValue || court.ClosingTime == TimeOnly.MinValue)
      {
        Console.WriteLine($"Court {court.Id} has invalid opening/closing times: {court.OpeningTime} / {court.ClosingTime}");
        return slots;
      }

      var start = court.OpeningTime;
      var end = court.ClosingTime;

      // Validar que apertura sea menor que cierre
      if (start >= end)
      {
        Console.WriteLine($"Court {court.Id} has invalid times: {start} >= {end}");
        return slots;
      }

      // Calcular cuántos slots de 90 minutos caben
      var totalMinutes = (end.ToTimeSpan() - start.ToTimeSpan()).TotalMinutes;
      var maxSlots = (int)(totalMinutes / 90);

      Console.WriteLine($"Court {court.Id}: {start} to {end}, total minutes: {totalMinutes}, max slots: {maxSlots}");

      // Generar exactamente el número de slots que caben
      for (int i = 0; i < maxSlots; i++)
      {
        var slotStart = start.AddMinutes(i * 90);
        var slotEnd = slotStart.AddMinutes(90);
        slots.Add((slotStart, slotEnd));
      }

      Console.WriteLine($"Court {court.Id} generated {slots.Count} slots");
      return slots;
    }
  }
}