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

  public class RegisterPaymentDto
  {
    public string PaymentMethod { get; set; } = "Efectivo";
  }

  public class MercadoPagoWebhookDto
  {
    public string Action { get; set; }
    public string ApiVersion { get; set; }
    public MercadoPagoDataDto Data { get; set; }
    public DateTime DateCreated { get; set; }
    public long Id { get; set; }
    public bool LiveMode { get; set; }
    public string Type { get; set; }
    public string UserId { get; set; }
  }

  public class MercadoPagoDataDto
  {
    public string Id { get; set; }
  }

  public class PaymentStatusUpdateDto
  {
    public int PaymentId { get; set; }
    public string MercadoPagoPaymentId { get; set; }
    public PaymentStatus NewStatus { get; set; }
    public string? Reason { get; set; }
  }
}