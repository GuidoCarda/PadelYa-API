using Microsoft.EntityFrameworkCore;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Payment;
using padelya_api.Models;
using LocalPayment = padelya_api.Models.Payment;
using LocalPaymentStatus = padelya_api.Constants.PaymentStatus;
using LocalPaymentType = padelya_api.Constants.PaymentType;
using System.Text.Json;
using padelya_api.DTOs.Booking;
using MercadoPago.Resource.Payment;
using padelya_api.Services.Email;

namespace padelya_api.Services
{
  public interface IPaymentService
  {
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto);
    Task<PaymentDto> GetPaymentByIdAsync(int id);
    Task<bool> UpdatePaymentStatusAsync(PaymentStatusUpdateDto dto);
    Task<LocalPaymentStatus> ProcessMercadoPagoWebhookAsync(MercadoPagoWebhookDto webhookData);
    Task<PaymentSummaryDto> GetSummaryAsync(string paymentId);
  }


  //  Type: representsThe target entity type(e.g., "booking", "lesson", "tournament")</param>
  //  <Id: represents the ID of that specific entity(e.g., bookingId 123)</param>
  public record PaymentTarget(string Type, int Id);

  public class PaymentService : IPaymentService
  {
    private readonly PadelYaDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailNotificationService _emailNotificationService;

    public PaymentService(PadelYaDbContext context, IConfiguration configuration, IEmailNotificationService emailNotificationService)
    {
      _context = context;
      _configuration = configuration;
      _emailNotificationService = emailNotificationService;
    }

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto)
    {
      var payment = new LocalPayment
      {
        Amount = dto.Amount,
        PaymentMethod = dto.PaymentMethod,
        PaymentStatus = LocalPaymentStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        TransactionId = dto.TransactionId,
        PaymentType = dto.PaymentType,
        PersonId = dto.PersonId,
        BookingId = dto.BookingId,
        LessonEnrollmentId = dto.LessonEnrollmentId,
        TournamentEnrollmentId = dto.TournamentEnrollmentId
      };

      _context.Payments.Add(payment);
      await _context.SaveChangesAsync();

      return new PaymentDto
      {
        Id = payment.Id,
        Amount = payment.Amount,
        PaymentMethod = payment.PaymentMethod,
        PaymentStatus = payment.PaymentStatus,
        CreatedAt = payment.CreatedAt,
        TransactionId = payment.TransactionId,
        PersonId = payment.PersonId
      };
    }

    public async Task<PaymentDto> GetPaymentByIdAsync(int id)
    {
      var payment = await _context.Payments.FindAsync(id);
      if (payment == null)
        throw new ArgumentException("Pago no encontrado");

      return new PaymentDto
      {
        Id = payment.Id,
        Amount = payment.Amount,
        PaymentMethod = payment.PaymentMethod,
        PaymentStatus = payment.PaymentStatus,
        CreatedAt = payment.CreatedAt,
        TransactionId = payment.TransactionId,
        PersonId = payment.PersonId
      };
    }

    public async Task<bool> UpdatePaymentStatusAsync(PaymentStatusUpdateDto dto)
    {
      var payment = await _context.Payments.FindAsync(dto.PaymentId);
      if (payment == null)
        return false;

      payment.PaymentStatus = dto.NewStatus;
      payment.TransactionId = dto.MercadoPagoPaymentId;

      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<LocalPaymentStatus> ProcessMercadoPagoWebhookAsync(MercadoPagoWebhookDto webhookData)
    {
      MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];

      var paymentId = webhookData.Data.Id;
      var client = new PaymentClient();
      var payment = await client.GetAsync(long.Parse(paymentId));

      Console.WriteLine($"payment.Id: {payment.Id} | status {payment.Status} \n\n\n  ");

      Console.WriteLine(JsonSerializer.Serialize(payment, new JsonSerializerOptions
      {
        WriteIndented = true // Enables pretty-printing
      }));

      var (target, targetId) = ParseExternalReference(payment);

      switch (target)
      {
        case "booking":
          await HandleBookingPayment(payment, targetId);
          break;
        case "lesson":
          break;
        case "tournament":
          break;
        default:
          throw new ArgumentException($"Unknown target entity: {target}");
      }

      var status = MapMercadoPagoStatus(payment.Status);

      return status;
    }

    private LocalPaymentStatus MapMercadoPagoStatus(string mercadoPagoStatus)
    {
      return mercadoPagoStatus?.ToLower() switch
      {
        "approved" => LocalPaymentStatus.Approved,
        "pending" => LocalPaymentStatus.Pending,
        "in_process" => LocalPaymentStatus.Pending,
        "rejected" => LocalPaymentStatus.Rejected,
        "cancelled" => LocalPaymentStatus.Rejected,
        "refunded" => LocalPaymentStatus.Rejected,
        _ => LocalPaymentStatus.Pending
      };
    }

    private static PaymentTarget ParseExternalReference(MercadoPago.Resource.Payment.Payment payment)
    {
      if (string.IsNullOrEmpty(payment.ExternalReference))
      {
        throw new ArgumentException("External reference is missing");
      }

      var parts = payment.ExternalReference.Split('_');

      if (parts.Length != 2)
      {
        throw new ArgumentException($"Invalid format: {payment.ExternalReference}");
      }

      var (target, targetIdStr) = (parts[0], parts[1]);

      if (!int.TryParse(targetIdStr, out var targetId))
      {
        throw new ArgumentException($"Invalid target ID: {targetIdStr}");
      }

      return new PaymentTarget(target, targetId);
    }

    public async Task<PaymentSummaryDto> GetSummaryAsync(string paymentId)
    {
      Console.WriteLine($"Getting summary for paymentId: {paymentId}");

      var payment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == paymentId);


      Console.WriteLine(JsonSerializer.Serialize(payment, new JsonSerializerOptions
      {
        WriteIndented = true // Enables pretty-printing
      }));

      if (payment == null)
        throw new Exception("Payment not found");

      var booking = await _context.Bookings
      .Include(b => b.CourtSlot)
      .Include(b => b.CourtSlot.Court)
      .Include(b => b.Payments)
      .Include(b => b.Person)
      .FirstOrDefaultAsync(b => b.Id == payment.BookingId);

      if (booking == null)
        throw new Exception("Booking not found");

      return new PaymentSummaryDto
      {
        TotalAmount = booking.CourtSlot.Court.BookingPrice,
        TotalPaid = booking.Payments.Sum(p => p.Amount),
        Booking = new BookingDto
        {
          Id = booking.Id,
          CourtSlotId = booking.CourtSlotId,
          CourtName = booking.CourtSlot.Court.Name,
          CourtType = booking.CourtSlot.Court.Type,
          PersonId = booking.PersonId,
          Status = booking.Status,
          StartTime = booking.CourtSlot.StartTime,
          Date = booking.CourtSlot.Date,
          DisplayStatus = booking.DisplayStatus
        }
      };
    }

    private async Task HandleBookingPayment(
      MercadoPago.Resource.Payment.Payment payment,
      int bookingId
    )
    {
      try
      {
        var booking = await _context.Bookings
                .Include(b => b.CourtSlot)
                  .ThenInclude(cs => cs.Court)
                .Include(b => b.Person)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking is null)
        {
          throw new Exception("Booking not found");
        }

        // Obtener el complejo para el email
        var complex = await _context.Set<Complex>()
                .FirstOrDefaultAsync(c => c.Id == booking.CourtSlot.Court.ComplexId);

        var bookingPrice = booking.CourtSlot.Court.BookingPrice;
        var amountPaid = payment.TransactionAmount ?? 0;

        var paymentType = GetPaymentType(bookingPrice, amountPaid);

        if (payment.Status == MercadoPago.Resource.Payment.PaymentStatus.Approved)
        {
          var personIdString = payment.Metadata?["person_id"] as string;
          if (string.IsNullOrEmpty(personIdString) || !int.TryParse(personIdString, out int personId))
            throw new Exception("PersonId not found or invalid");

          var newPayment = new LocalPayment
          {
            Amount = payment.TransactionAmount ?? 0,
            PaymentMethod = payment.PaymentTypeId,
            PaymentStatus = LocalPaymentStatus.Approved,
            CreatedAt = payment.DateApproved ?? DateTime.UtcNow,
            TransactionId = payment.Id.ToString()!,
            BookingId = bookingId,
            PersonId = personId,
            PaymentType = paymentType
          };
          _context.Payments.Add(newPayment);

          booking.Status = bookingPrice == amountPaid
            ? BookingStatus.ReservedPaid
            : BookingStatus.ReservedDeposit;
          booking.CourtSlot.Status = CourtSlotStatus.Active;

          await _context.SaveChangesAsync();

          // Enviar email de confirmación de reserva
          await _emailNotificationService.SendBookingConfirmationAsync(booking, complex, amountPaid);
        }

        if (payment.Status == MercadoPago.Resource.Payment.PaymentStatus.Rejected)
        {

          var newPayment = new LocalPayment
          {
            Amount = payment.TransactionAmount ?? 0,
            PaymentMethod = payment.PaymentMethodId,
            PaymentStatus = LocalPaymentStatus.Rejected,
            CreatedAt = payment.DateCreated ?? DateTime.UtcNow,
            TransactionId = payment.Id.ToString()!,
            BookingId = bookingId,
            PersonId = booking.PersonId,
            PaymentType = paymentType,
          };
          _context.Payments.Add(newPayment);

          if (booking.CourtSlot.IsExpired)
          {
            booking.Status = BookingStatus.CancelledBySystem;
            booking.CancellationReason = "El pago fue rechazado";
            booking.CourtSlot.Status = CourtSlotStatus.Cancelled;
          }

          await _context.SaveChangesAsync();
        }


        //TODO: Handle pending status
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }


    private static LocalPaymentType GetPaymentType(decimal price, decimal amountPaid)
    {
      LocalPaymentType type;
      if (Math.Abs(amountPaid - price) < 0.01m)
        type = LocalPaymentType.Total;
      else if (Math.Abs(amountPaid - (price * 0.5m)) < 0.01m)
        type = LocalPaymentType.Deposit;
      else
        type = LocalPaymentType.Balance;

      return type;
    }
  }



  public class PaymentSummaryDto
  {
    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public BookingDto Booking { get; set; }
  }

  public class CreatePaymentDto
  {
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public PaymentType PaymentType { get; set; }
    public int PersonId { get; set; }
    public int? BookingId { get; set; }
    public int? LessonEnrollmentId { get; set; }
    public int? TournamentEnrollmentId { get; set; }
  }
}
