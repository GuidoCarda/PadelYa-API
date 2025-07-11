namespace padelya_api.Constants
{
  public enum BookingStatus
  {
    ReservedPaid,      // Reservado y pagado completamente
    ReservedDeposit,   // Reservado con seña
    CancelledByClient,
    CancelledByAdmin,
  }
}
