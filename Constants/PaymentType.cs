namespace padelya_api.Constants
{
  public enum PaymentType
  {
    Deposit, // Seña
    Balance, // Saldo restante
    Total, // Pago total
  }

  public enum PaymentStatus
  {
    Pending,
    Approved,
    Rejected
  }
}