using padelya_api.Constants;

namespace padelya_api.DTOs.Payment
{
  public class PaymentDto
  {
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TransactionId { get; set; }
    public int PersonId { get; set; }
  }
}