using Microsoft.EntityFrameworkCore;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Payment;
using LocalPayment = padelya_api.Models.Payment;
using LocalPaymentStatus = padelya_api.Constants.PaymentStatus;
using LocalPaymentType = padelya_api.Constants.PaymentType;

namespace padelya_api.Services
{
  public interface IPaymentService
  {
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto);
    Task<PaymentDto> GetPaymentByIdAsync(int id);
    Task<bool> UpdatePaymentStatusAsync(PaymentStatusUpdateDto dto);
    Task<LocalPaymentStatus> ProcessMercadoPagoWebhookAsync(MercadoPagoWebhookDto webhookData);
  }

  public class PaymentService : IPaymentService
  {
    private readonly PadelYaDbContext _context;
    private readonly IConfiguration _configuration;

    public PaymentService(PadelYaDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
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

      var localPayment = await _context.Payments
          .FirstOrDefaultAsync(p => p.TransactionId == paymentId);

      var newStatus = MapMercadoPagoStatus(payment.Status);

      if (localPayment == null && newStatus == LocalPaymentStatus.Approved)
      {
        int? bookingId = null;
        if (!string.IsNullOrEmpty(payment.ExternalReference) && int.TryParse(payment.ExternalReference, out var parsedBookingId))
        {
          bookingId = parsedBookingId;
        }

        var newPayment = new LocalPayment
        {
          Amount = payment.TransactionAmount ?? 0,
          PaymentMethod = payment.PaymentMethodId,
          PaymentStatus = LocalPaymentStatus.Approved,
          CreatedAt = payment.DateApproved ?? DateTime.UtcNow,
          TransactionId = paymentId,
          BookingId = bookingId,
          // TODO: Asociar PersonId correctamente segun usuario
          PersonId = 1,
          PaymentType = LocalPaymentType.Total // o Deposit, según corresponda
        };
        _context.Payments.Add(newPayment);
        await _context.SaveChangesAsync();
      }
      // Si ya existe, solo actualizamos el estado
      else if (localPayment != null)
      {
        localPayment.PaymentStatus = newStatus;
        await _context.SaveChangesAsync();
      }

      return newStatus;
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
