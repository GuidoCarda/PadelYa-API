using padelya_api.Models;
using padelya_api.Models.Repair;

namespace padelya_api.Services.Email;

/// <summary>
/// Servicio centralizado para el envío de notificaciones por email.
/// Encapsula toda la lógica de negocio relacionada con emails.
/// </summary>
public interface IEmailNotificationService
{
  // ========== Auth ==========

  /// <summary>
  /// Envía email con la nueva contraseña generada para recuperación.
  /// </summary>
  Task SendPasswordRecoveryAsync(string email, string userName, string newPassword);

  /// <summary>
  /// Envía email con la contraseña de respaldo.
  /// </summary>
  Task SendBackupPasswordAsync(string email, string userName, string backupPassword, DateTime expirationDate);

  // ========== Booking ==========

  /// <summary>
  /// Envía email de confirmación de reserva.
  /// </summary>
  Task SendBookingConfirmationAsync(Booking booking, Complex? complex, decimal amountPaid);

  /// <summary>
  /// Envía email de cancelación de reserva.
  /// </summary>
  Task SendBookingCancellationAsync(Booking booking, string? cancellationReason);

  // ========== Ecommerce ==========

  /// <summary>
  /// Envía email de confirmación de compra.
  /// </summary>
  Task SendPurchaseConfirmationAsync(string email, string userName, string orderNumber, DateTime purchaseDate,
      List<(string Name, int Quantity, decimal UnitPrice)> items, decimal subtotal, decimal tax, decimal total, string? deliveryAddress);

  // ========== Repair ==========

  /// <summary>
  /// Envía email de reparación lista para pickup.
  /// </summary>
  Task SendRepairReadyForPickupAsync(Repair repair);
}

