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
using MercadoPago.Resource.Preference;
using MercadoPago.Client.Preference;
using MercadoPago.Config;

namespace padelya_api.Services
{

  public class ReservationInitPointDto
  {
    public string init_point { get; set; }
  }

  public class BookingService : IBookingService
  {
    private readonly PadelYaDbContext _context;
    private readonly ICourtSlotService _courtSlotService;
    private readonly IConfiguration _configuration;

    private static readonly TimeSpan BookingExpirationTime = TimeSpan.FromMinutes(3);

    public BookingService(PadelYaDbContext context, ICourtSlotService courtSlotService, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
      _courtSlotService = courtSlotService;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync(
      string? email = null,
      string? status = null,
      string? startDate = null,
      string? endDate = null)
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

      if (!string.IsNullOrEmpty(startDate))
      {
        if (DateTime.TryParseExact(startDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var start))
        {
          query = query.Where(b => b.CourtSlot.Date.Date >= start.Date);
        }
        else
        {
          throw new ArgumentException("El formato de fecha debe ser YYYY-MM-DD");
        }

      }

      if (!string.IsNullOrEmpty(endDate))
      {
        if (DateTime.TryParseExact(endDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var end))
        {
          query = query.Where(b => b.CourtSlot.Date.Date <= end.Date);
        }
        else
        {
          throw new ArgumentException("El formato de fecha debe ser YYYY-MM-DD");
        }
      }

      if (!string.IsNullOrEmpty(status))
      {
        if (Enum.TryParse<BookingStatus>(status, true, out var statusEnum))
        {
          query = query.Where(b => b.Status == statusEnum);
        }
      }

      var bookings = await query
        .OrderByDescending(bk => bk.CourtSlot.Date)
        .ThenBy(bk => bk.CourtSlot.StartTime)
        .ToListAsync();

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

    public async Task<BookingDto?> GetByIdAsync(int id)
    {
      var booking = await _context.Bookings
          .Include(bk => bk.CourtSlot)
          .Include(b => b.CourtSlot.Court)
          .Include(bk => bk.Payments)
          .Include(bk => bk.Person)
          .FirstOrDefaultAsync(bk => bk.Id == id);

      if (booking == null) return null;

      return new BookingDto
      {
        Id = booking.Id,
        CourtSlotId = booking.CourtSlotId,
        PersonId = booking.PersonId,
        Status = booking.Status,
        DisplayStatus = booking.DisplayStatus,

        // Información del slot/cancha
        Date = booking.CourtSlot.Date,
        StartTime = booking.CourtSlot.StartTime,
        EndTime = booking.CourtSlot.EndTime,
        CourtId = booking.CourtSlot.Court.Id,
        CourtName = booking.CourtSlot.Court.Name,
        CourtType = booking.CourtSlot.Court.Type,


        // Información del usuario (obtener desde la tabla User)
        UserName = _context.Users.FirstOrDefault(u => u.PersonId == booking.PersonId)?.Name ?? "",
        UserSurname = _context.Users.FirstOrDefault(u => u.PersonId == booking.PersonId)?.Surname ?? "",
        UserEmail = _context.Users.FirstOrDefault(u => u.PersonId == booking.PersonId)?.Email ?? "",

        // Información de pagos
        Payments = booking.Payments.Select(p => new PaymentDto
        {
          Id = p.Id,
          Amount = p.Amount,
          PaymentMethod = p.PaymentMethod,
          PaymentStatus = p.PaymentStatus,
          CreatedAt = p.CreatedAt,
          TransactionId = p.TransactionId,
          PersonId = p.PersonId
        }).ToList(),
        TotalPaid = booking.Payments.Sum(p => p.Amount),
        TotalAmount = booking.CourtSlot.Court.BookingPrice
      };
    }

    public async Task<BookingDto> CreateAsync(BookingCreateDto dto)
    {
      var existsPerson = await _context.Set<Person>().AnyAsync(p => p.Id == dto.PersonId);
      if (!existsPerson)
        throw new Exception("Person not found.");

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

    public async Task<bool> CancelAsync(int id, CancelBookingDto dto)
    {
      var booking = await _context.Bookings
        .Include(bk => bk.CourtSlot)
        .FirstOrDefaultAsync(bk => bk.Id == id);

      if (booking == null) return false;

      booking.Status = dto.CancelledBy == "admin"
        ? BookingStatus.CancelledByAdmin
        : BookingStatus.CancelledByClient;

      booking.CancelledBy = dto.CancelledBy;
      booking.CancellationReason = dto.Reason;
      booking.CancelledAt = DateTime.Now;

      booking.CourtSlot.Status = CourtSlotStatus.Cancelled;

      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> CancelExpiredAsync(int id)
    {
      var booking = await _context.Bookings
        .Include(b => b.CourtSlot)
        .FirstOrDefaultAsync(b => b.Id == id);

      if (booking == null) return false;

      booking.Status = BookingStatus.CancelledBySystem;
      booking.CancellationReason = "No se concreto el pago en tiempo";
      booking.CancelledBy = "system";
      booking.CancelledAt = DateTime.Now;
      booking.CourtSlot.Status = CourtSlotStatus.Cancelled;

      await _context.SaveChangesAsync();

      return true;
    }

    public async Task<BookingResponseDto> CreateAdminBookingAsync(BookingCreateDto dto)
    {
      Console.WriteLine($"Iniciando creacion de reserva de administrador: CourtId={dto.CourtId}, Date={dto.Date}, PersonId={dto.PersonId}");

      var existsPerson = await _context.Set<Person>().AnyAsync(p => p.Id == dto.PersonId);

      if (!existsPerson)
        throw new Exception("No se encontró el cliente.");

      // Clean expired pending slots for now manually, later on we could use a cron job to do this or another way to handle this.
      await CleanExpiredPendingSlotsAsync();

      var slot = await _courtSlotService.CreateSlotIfAvailableAsync(dto.CourtId, dto.Date, dto.StartTime, dto.StartTime.AddMinutes(90));

      if (slot == null)
        throw new Exception("El turno no está disponible.");

      var booking = new Booking
      {
        CourtSlotId = slot.Id,
        PersonId = dto.PersonId,
        Status = dto.PaymentType == PaymentType.Deposit
          ? BookingStatus.ReservedDeposit
          : BookingStatus.ReservedPaid,
      };

      _context.Bookings.Add(booking);
      await _context.SaveChangesAsync();

      var payment = new Payment
      {
        Amount = dto.PaymentType == PaymentType.Deposit
          ? booking.CourtSlot.Court.BookingPrice * 0.5m
          : booking.CourtSlot.Court.BookingPrice,
        PaymentMethod = dto.PaymentMethod,
        PaymentStatus = PaymentStatus.Approved,
        BookingId = booking.Id,
        PaymentType = dto.PaymentType,
        PersonId = booking.PersonId,
        CreatedAt = DateTime.Now,
        TransactionId = "simulado",
      };


      _context.Payments.Add(payment);
      await _context.SaveChangesAsync();


      return new BookingResponseDto
      {
        Payment = new PaymentDto
        {
          Id = payment.Id,
          Amount = payment.Amount,
          PaymentMethod = payment.PaymentMethod,
          PaymentStatus = payment.PaymentStatus,
          CreatedAt = payment.CreatedAt,
          TransactionId = payment.TransactionId,
          PersonId = payment.PersonId
        },
        Booking = new BookingDto
        {
          Id = booking.Id,
          CourtSlotId = booking.CourtSlotId,
          PersonId = booking.PersonId,
          Status = booking.Status,
        }
      };
    }


    public async Task<ReservationInitPointDto> CreateWithPaymentAsync(BookingReserveWithPaymentDto dto)
    {
      Console.WriteLine($"Iniciando reserva con pago: CourtId={dto.CourtId}, Date={dto.Date}, PersonId={dto.PersonId}");

      var existsPerson = await _context.Set<Person>().AnyAsync(p => p.Id == dto.PersonId);
      if (!existsPerson)
        throw new Exception("No se encontró el cliente.");

      var endTime = dto.StartTime.AddMinutes(90);

      var court = await _context.Courts.FirstOrDefaultAsync(c => c.Id == dto.CourtId);

      if (court == null)
        throw new Exception("No se encontró la cancha.");

      // Create slot with pending status (15 minutes to complete the payment)
      var slot = await CreatePendingSlotAsync(dto.CourtId, dto.Date, dto.StartTime, endTime);
      Console.WriteLine($"Slot creado: Id={slot.Id}");

      decimal amount = dto.PaymentType == PaymentType.Deposit
          ? court.BookingPrice * 0.5m
          : court.BookingPrice;

      Console.WriteLine($"Monto de reserva calculado: {amount}");

      // Create booking with pending status
      var booking = new Booking
      {
        CourtSlotId = slot.Id,
        PersonId = dto.PersonId,
        Status = BookingStatus.PendingPayment,
        PreferenceId = null
      };

      _context.Bookings.Add(booking);
      await _context.SaveChangesAsync();

      Console.WriteLine($"Booking creado: Id={booking.Id}");

      var preference = await CreateMercadoPagoPreference(booking, amount, dto.PaymentType);

      if (preference == null)
        throw new Exception("Preference not created");

      booking.PreferenceId = preference.Id;
      await _context.SaveChangesAsync();

      var response = new ReservationInitPointDto
      {
        init_point = preference.InitPoint
      };

      return response;
    }


    private async Task<Preference> CreateMercadoPagoPreference(Booking booking, decimal amount, PaymentType paymentType)
    {
      MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];

      Console.WriteLine($"Creando preferencia de pago para reserva {booking.Id} con monto {amount}");

      var externalReference = $"booking_{booking.Id}";

      var request = new PreferenceRequest
      {
        Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = "Reserva de cancha turno de padel",
                    Quantity = 1,
                    CurrencyId = "ARS",
                    UnitPrice = amount
                }
            },
        ExternalReference = externalReference,
        BackUrls = new PreferenceBackUrlsRequest
        {
          Success = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/success",
          Failure = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/failure",
          Pending = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/pending"
        },
        Metadata = new Dictionary<string, object>
        {
          ["booking_id"] = booking.Id.ToString(),
          ["person_id"] = booking.PersonId.ToString(),
          ["court_id"] = booking.CourtSlot.CourtId.ToString(),
          ["payment_type"] = paymentType.ToString(),
          ["date"] = booking.CourtSlot.Date.ToString(),
          ["start_time"] = booking.CourtSlot.StartTime.ToString(),
          ["end_time"] = booking.CourtSlot.EndTime.ToString()
        },
        AutoReturn = "approved",
        Expires = true,
        ExpirationDateTo = DateTime.UtcNow.Add(BookingExpirationTime),
        DateOfExpiration = DateTime.UtcNow.Add(BookingExpirationTime),

        PaymentMethods = new()
        {
          Installments = 1,
          ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
          {
              new PreferencePaymentTypeRequest
              {
                  Id = "ticket",
            },
              new PreferencePaymentTypeRequest
              {
                  Id = "credit_card",
              },
          },
        },
      };

      var client = new PreferenceClient();
      Preference preference = await client.CreateAsync(request);

      return preference;
    }

    private async Task<CourtSlot> CreatePendingSlotAsync(int courtId, DateTime date, TimeOnly start, TimeOnly end)
    {

      var existingSlot = await _context.CourtSlots
        .FirstOrDefaultAsync(cs =>
            cs.CourtId == courtId &&
            cs.Date == date &&
            cs.StartTime == start &&
            cs.EndTime == end &&
            (cs.Status == CourtSlotStatus.Active ||
             (cs.Status == CourtSlotStatus.Pending && cs.ExpiresAt > DateTime.UtcNow)));

      if (existingSlot != null)
        throw new Exception("Ese turno ya está ocupado.");

      await CleanExpiredPendingSlotsAsync();

      var slot = new CourtSlot
      {
        CourtId = courtId,
        Date = date,
        StartTime = start,
        EndTime = end,
        Status = CourtSlotStatus.Pending,
        ExpiresAt = DateTime.UtcNow.Add(BookingExpirationTime)
      };


      _context.CourtSlots.Add(slot);
      await _context.SaveChangesAsync();
      return slot;
    }

    private async Task CleanExpiredPendingSlotsAsync()
    {
      var expiredSlots = await _context.CourtSlots
        .Include(cs => cs.Booking)
        .Where(cs => cs.Status == CourtSlotStatus.Pending &&
                     cs.ExpiresAt < DateTime.UtcNow)
        .ToListAsync();


      foreach (var slot in expiredSlots)
      {
        if (slot.Booking != null)
        {
          slot.Booking.Status = BookingStatus.CancelledBySystem;
        }
        slot.Status = CourtSlotStatus.Cancelled;
      }

      await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CourtAvailabilityDto>> GetDailyAvailabilityAsync(DateTime date)
    {
      var courts = await _context.Courts
        .Where(c => c.CourtStatus == CourtStatus.Available)
        .ToListAsync();

      var occupiedSlots = await _context.CourtSlots
          .Where(cs => cs.Date.Date == date.Date &&
            (cs.Status == CourtSlotStatus.Active ||
            (cs.Status == CourtSlotStatus.Pending && cs.ExpiresAt > DateTime.UtcNow)))
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


    public async Task<BookingDto?> RegisterPaymentAsync(int id, RegisterPaymentDto paymentDto)
    {
      var booking = await _context.Bookings
          .Include(bk => bk.CourtSlot)
          .Include(b => b.CourtSlot.Court)
          .Include(bk => bk.Payments)
          .Include(bk => bk.Person)
          .FirstOrDefaultAsync(bk => bk.Id == id);

      if (booking == null) throw new Exception("Reserva no encontrada.");

      Console.WriteLine(booking.PendingAmount);

      decimal pendingAmount = booking.CourtSlot.Court.BookingPrice - booking.Payments.Sum(p => p.Amount);

      // TotalPaid = booking.Payments.Sum(p => p.Amount),
      //   TotalAmount = booking.CourtSlot.Court.BookingPrice

      if (pendingAmount <= 0)
        throw new Exception("La reserva ya está pagada completamente.");

      var payment = new Payment
      {
        BookingId = id,
        Amount = pendingAmount,
        CreatedAt = DateTime.Now,
        PaymentStatus = PaymentStatus.Approved,
        TransactionId = "simulado",
        PaymentType = PaymentType.Balance,
        PaymentMethod = paymentDto.PaymentMethod,
        PersonId = booking.PersonId
      };

      booking.Status = BookingStatus.ReservedPaid;

      _context.Payments.Add(payment);
      await _context.SaveChangesAsync();

      var updatedBooking = new BookingDto
      {
        Id = booking.Id,
        CourtSlotId = booking.CourtSlotId,
        PersonId = booking.PersonId,
        Status = booking.Status,
        DisplayStatus = booking.DisplayStatus,

        // Información del slot/cancha
        Date = booking.CourtSlot.Date,
        StartTime = booking.CourtSlot.StartTime,
        EndTime = booking.CourtSlot.EndTime,
        CourtId = booking.CourtSlot.Court.Id,
        CourtName = booking.CourtSlot.Court.Name,
        CourtType = booking.CourtSlot.Court.Type,

        // Información del usuario (obtener desde la tabla User)
        UserName = _context.Users.FirstOrDefault(u => u.PersonId == booking.PersonId)?.Name ?? "",
        UserSurname = _context.Users.FirstOrDefault(u => u.PersonId == booking.PersonId)?.Surname ?? "",
        UserEmail = _context.Users.FirstOrDefault(u => u.PersonId == booking.PersonId)?.Email ?? "",

        // Información de pagos
        Payments = booking.Payments.Select(p => new PaymentDto
        {
          Id = p.Id,
          Amount = p.Amount,
          PaymentMethod = p.PaymentMethod,
          PaymentStatus = p.PaymentStatus,
          CreatedAt = p.CreatedAt,
          TransactionId = p.TransactionId,
          PersonId = p.PersonId
        }).ToList(),
        TotalPaid = booking.Payments.Sum(p => p.Amount),
        TotalAmount = booking.CourtSlot.Court.BookingPrice
      };

      return updatedBooking;
    }



    public async Task<List<BookingDto>> GetUserBookingsAsync(int userId, string? status = null)
    {
      var personId = await _context.Users
          .Where(u => u.Id == userId)
          .Select(u => u.PersonId)
          .FirstOrDefaultAsync();

      if (!personId.HasValue)
        return new List<BookingDto>();

      var query = _context.Bookings
          .Include(b => b.CourtSlot)
          .Include(b => b.CourtSlot.Court)
          .Include(b => b.Person)
          .Include(b => b.Payments)
          .Where(b => b.PersonId == personId.Value)
          .AsQueryable();

      // Apply status filter if provided
      if (!string.IsNullOrEmpty(status))
      {
        if (Enum.TryParse<BookingStatus>(status, true, out var statusEnum))
        {
          query = query.Where(b => b.Status == statusEnum);
        }
      }

      var bookings = await query.OrderByDescending(bk => bk.Id).ToListAsync();

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
      }).ToList();
    }

    public async Task<List<BookingDto>> GetBookingsByPersonIdAsync(int personId, string? status = null)
    {
      var query = _context.Bookings
          .Include(b => b.CourtSlot)
          .Include(b => b.CourtSlot.Court)
          .Include(b => b.Person)
          .Include(b => b.Payments)
          .Where(b => b.PersonId == personId)
          .AsQueryable();

      if (!string.IsNullOrEmpty(status))
      {
        if (Enum.TryParse<BookingStatus>(status, true, out var statusEnum))
        {
          query = query.Where(b => b.Status == statusEnum);
        }
      }

      var bookings = await query.OrderByDescending(bk => bk.Id).ToListAsync();

      return bookings.Select(b => new BookingDto
      {
        Id = b.Id,
        CourtSlotId = b.CourtSlotId,
        PersonId = b.PersonId,
        Status = b.Status,
        DisplayStatus = b.DisplayStatus,

        Date = b.CourtSlot.Date,
        StartTime = b.CourtSlot.StartTime,
        EndTime = b.CourtSlot.EndTime,
        CourtId = b.CourtSlot.Court.Id,
        CourtName = b.CourtSlot.Court.Name,
        CourtType = b.CourtSlot.Court.Type,

        UserName = _context.Users.FirstOrDefault(u => u.PersonId == b.PersonId)?.Name ?? "",
        UserSurname = _context.Users.FirstOrDefault(u => u.PersonId == b.PersonId)?.Surname ?? "",
        UserEmail = _context.Users.FirstOrDefault(u => u.PersonId == b.PersonId)?.Email ?? "",

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
      }).ToList();
    }

    // Helper para generar slots posibles de 90 minutos
    private List<(TimeOnly start, TimeOnly end)> GenerateSlotsForCourt(Models.Court court, DateTime date)
    {
      var slots = new List<(TimeOnly, TimeOnly)>();

      // Validación y cálculo de rango soportando horarios nocturnos (cierre 00:00 del día siguiente)
      if (court.OpeningTime == TimeOnly.MinValue)
      {
        Console.WriteLine($"Court {court.Id} has invalid opening time (MinValue): {court.OpeningTime}");
        return slots;
      }

      var open = court.OpeningTime;
      var close = court.ClosingTime;

      var openTs = open.ToTimeSpan();
      var closeTs = close.ToTimeSpan();

      // Duración del negocio; si cierra al día siguiente (ej. 07:30 -> 00:00), envolver
      var businessSpan = closeTs > openTs
        ? closeTs - openTs
        : (TimeSpan.FromDays(1) - openTs) + closeTs;

      if (businessSpan <= TimeSpan.Zero || businessSpan > TimeSpan.FromDays(1))
      {
        Console.WriteLine($"Court {court.Id} has invalid business span: {businessSpan}");
        return slots;
      }

      // Número de slots de 90 minutos que entran en el rango (fin exclusivo)
      var slotLength = TimeSpan.FromMinutes(90);
      var maxSlots = (int)(businessSpan.TotalMinutes / slotLength.TotalMinutes);

      Console.WriteLine($"Court {court.Id}: {open} to {close} (span {businessSpan}), max slots: {maxSlots}");

      // Generación modular sobre 24h para soportar cruce de medianoche
      for (int i = 0; i < maxSlots; i++)
      {
        var slotStartTs = openTs + TimeSpan.FromMinutes(i * 90);
        if (slotStartTs >= TimeSpan.FromDays(1)) slotStartTs -= TimeSpan.FromDays(1);
        var slotEndTs = slotStartTs + slotLength;
        if (slotEndTs >= TimeSpan.FromDays(1)) slotEndTs -= TimeSpan.FromDays(1);

        var slotStart = TimeOnly.FromTimeSpan(slotStartTs);
        var slotEnd = TimeOnly.FromTimeSpan(slotEndTs);
        slots.Add((slotStart, slotEnd));
      }

      Console.WriteLine($"Court {court.Id} generated {slots.Count} slots");
      return slots;
    }
  }
}