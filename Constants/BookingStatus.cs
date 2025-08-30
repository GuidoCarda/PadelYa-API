namespace padelya_api.Constants
{
  public enum BookingStatus
  {
    PendingPayment,    // Pendiente de pago
    ReservedPaid,      // Reservado y pagado completamente
    ReservedDeposit,   // Reservado con seña
    CancelledByClient,
    CancelledByAdmin,
    CancelledBySystem, // Expirado
  }
}
