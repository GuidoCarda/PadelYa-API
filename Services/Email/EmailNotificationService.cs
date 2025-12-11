using padelya_api.Models;
using padelya_api.Models.Repair;
using padelya_api.Services.Email.Templates.Auth;
using padelya_api.Services.Email.Templates.Booking;
using padelya_api.Services.Email.Templates.Ecommerce;
using padelya_api.Services.Email.Templates.Repair;

namespace padelya_api.Services.Email;

/// <summary>
/// Implementación del servicio de notificaciones por email.
/// Centraliza toda la lógica de envío de emails de la aplicación.
/// </summary>
public class EmailNotificationService : IEmailNotificationService
{
  private readonly IEmailService _emailService;
  private readonly ILogger<EmailNotificationService> _logger;

  public EmailNotificationService(IEmailService emailService, ILogger<EmailNotificationService> logger)
  {
    _emailService = emailService;
    _logger = logger;
  }

  // ========== Auth ==========

  public async Task SendPasswordRecoveryAsync(string email, string userName, string newPassword)
  {
    try
    {
      var template = new PasswordRecoveryTemplate();
      var data = new PasswordRecoveryData(userName, newPassword);
      await _emailService.SendTemplatedEmailAsync(email, template, data);
      _logger.LogInformation("Email de recuperación de contraseña enviado a {Email}", email);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error enviando email de recuperación de contraseña a {Email}", email);
      throw;
    }
  }

  public async Task SendBackupPasswordAsync(string email, string userName, string backupPassword, DateTime expirationDate)
  {
    try
    {
      var template = new BackupPasswordTemplate();
      var data = new BackupPasswordData(userName, backupPassword, expirationDate);
      await _emailService.SendTemplatedEmailAsync(email, template, data);
      _logger.LogInformation("Email de contraseña de respaldo enviado a {Email}", email);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error enviando email de contraseña de respaldo a {Email}", email);
      throw;
    }
  }

  // ========== Booking ==========

  public async Task SendBookingConfirmationAsync(Booking booking, Complex? complex, decimal amountPaid)
  {
    try
    {
      var template = new BookingConfirmationTemplate();
      var data = new BookingConfirmationData(
          UserName: booking.Person.Name,
          CourtName: booking.CourtSlot.Court.Name,
          Date: booking.CourtSlot.Date,
          StartTime: booking.CourtSlot.StartTime.ToTimeSpan(),
          EndTime: booking.CourtSlot.EndTime.ToTimeSpan(),
          ComplexName: complex?.Name ?? "PadelYa",
          ComplexAddress: complex?.Address,
          TotalPrice: amountPaid,
          BookingCode: $"RES-{booking.Id:D6}"
      );

      await _emailService.SendTemplatedEmailAsync(booking.Person.Email, template, data);
      _logger.LogInformation("Email de confirmación de reserva enviado a {Email} para reserva {BookingId}",
          booking.Person.Email, booking.Id);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error enviando email de confirmación de reserva a {Email}", booking.Person.Email);
      // No relanzamos la excepción para no afectar el flujo de pago
    }
  }

  public async Task SendBookingCancellationAsync(Booking booking, string? cancellationReason)
  {
    try
    {
      Console.WriteLine("Enviando email de cancelación de reserva...");

      // log booking details
      Console.WriteLine($"Booking ID: {booking.Id}, Email: {booking.Person.Email}, Court: {booking.CourtSlot.Court.Name}, Date: {booking.CourtSlot.Date}");


      var template = new BookingCancellationTemplate();
      var data = new BookingCancellationData(
          UserName: booking.Person.Name,
          CourtName: booking.CourtSlot.Court.Name,
          Date: booking.CourtSlot.Date,
          StartTime: booking.CourtSlot.StartTime.ToTimeSpan(),
          EndTime: booking.CourtSlot.EndTime.ToTimeSpan(),
          BookingCode: $"RES-{booking.Id:D6}",
          CancellationReason: cancellationReason
      );

      await _emailService.SendTemplatedEmailAsync(booking.Person.Email, template, data);
      _logger.LogInformation("Email de cancelación de reserva enviado a {Email} para reserva {BookingId}",
          booking.Person.Email, booking.Id);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error enviando email de cancelación de reserva a {Email}", booking.Person.Email);
      // No relanzamos la excepción para no afectar el flujo de cancelación
    }
  }

  public async Task SendRepairReadyForPickupAsync(Repair repair)
  {
    try
    {
      var template = new RepairReadyForPickupTemplate();
      var data = new RepairReadyForPickupData(
          UserName: repair.Person.Name,
          RacketBrand: repair.Racket.Brand,
          RacketModel: repair.Racket.Model,
          RepairCode: repair.Id.ToString(),
          DamageDescription: repair.DamageDescription,
          Price: repair.Price,
          RepairNotes: repair.RepairNotes
      );

      await _emailService.SendTemplatedEmailAsync(repair.Person.Email, template, data);
      _logger.LogInformation("Email de reparación lista para pickup enviado a {Email} para reparación {RepairId}",
          repair.Person.Email, repair.Id);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error enviando email de reparación lista para pickup a {Email}", repair.Person.Email);
      throw;
    }
  }

  // ========== Ecommerce ==========

  public async Task SendPurchaseConfirmationAsync(
      string email,
      string userName,
      string orderNumber,
      DateTime purchaseDate,
      List<(string Name, int Quantity, decimal UnitPrice)> items,
      decimal subtotal,
      decimal tax,
      decimal total,
      string? deliveryAddress)
  {
    try
    {
      var template = new PurchaseConfirmationTemplate();
      var purchaseItems = items.Select(i => new PurchaseItem(i.Name, i.Quantity, i.UnitPrice)).ToList();
      var data = new PurchaseConfirmationData(
          UserName: userName,
          OrderNumber: orderNumber,
          PurchaseDate: purchaseDate,
          Items: purchaseItems,
          Subtotal: subtotal,
          Tax: tax,
          Total: total,
          DeliveryAddress: deliveryAddress
      );

      await _emailService.SendTemplatedEmailAsync(email, template, data);
      _logger.LogInformation("Email de confirmación de compra enviado a {Email} para orden {OrderNumber}",
          email, orderNumber);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error enviando email de confirmación de compra a {Email}", email);
      throw;
    }
  }
}

