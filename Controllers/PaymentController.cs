using Microsoft.AspNetCore.Mvc;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using padelya_api.Services;
using padelya_api.DTOs.Payment;
using System.Text.Json;
using System.Text.Json.Serialization;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
  private readonly IConfiguration _configuration;
  private readonly IPaymentService _paymentService;

  public PaymentsController(IConfiguration configuration, IPaymentService paymentService)
  {
    _configuration = configuration;
    _paymentService = paymentService;
  }

  [HttpPost("create-preference")]
  public async Task<IActionResult> CreatePreference([FromBody] CreatePreferenceDto dto)
  {
    Console.WriteLine(dto);

    MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];

    // DOCS:  https://www.mercadopago.com.ar/developers/es/docs/vtex/payments-configuration/checkout-pro/exclude-payment-types-methods

    var paymentMethods = new PreferencePaymentMethodsRequest
    {
      ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
      {
        new PreferencePaymentTypeRequest
        {
          Id = "ticket",
        },
        new PreferencePaymentTypeRequest
        {
          Id = "credit_card",
        }
      },
      Installments = 1,
    };


    var request = new PreferenceRequest
    {
      Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = dto.Title,
                    Quantity = dto.Quantity,
                    CurrencyId = "ARS",
                    UnitPrice = dto.UnitPrice
                }
            },
      Metadata = new Dictionary<string, object>
      {
        ["booking_id"] = "test"
      },
      BackUrls = new PreferenceBackUrlsRequest
      {
        Success = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/success",
        Failure = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/failure",
        Pending = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/pending"
      },
      AutoReturn = "approved",
      // set preference valid for 10 minutes 
    };

    // ExpirationDateTo = DateTime.UtcNow.AddMinutes(10),
    // PaymentMethods = paymentMethods,
    var client = new PreferenceClient();
    Preference preference = await client.CreateAsync(request);

    return Ok(new
    {
      init_point = preference.InitPoint
    });
  }

  [HttpPost("webhook")]
  public async Task<IActionResult> MercadoPagoWebhook()
  {
    Console.WriteLine("Webhook request received");
    try
    {
      using var reader = new StreamReader(Request.Body);
      var body = await reader.ReadToEndAsync();

      var webhookData = JsonSerializer.Deserialize<MercadoPagoWebhookDto>(
          body,
          new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
      );

      Console.WriteLine($"webhookData: {JsonSerializer.Serialize(webhookData)}");
      if (webhookData == null || webhookData.Data == null || string.IsNullOrEmpty(webhookData.Data.Id))
      {
        Console.WriteLine("Webhook mal formado o faltan datos.");
        return BadRequest("Webhook mal formado o faltan datos.");
      }

      var paymentStatus = await _paymentService.ProcessMercadoPagoWebhookAsync(webhookData);

      Console.WriteLine($"Webhook procesado. Estado del pago: {paymentStatus}");

      return Ok(new { message = "Webhook procesado correctamente", status = paymentStatus });
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error procesando webhook: {ex.Message}");
      return BadRequest(new { error = ex.Message });
    }
  }


  [HttpGet("summary")]
  public async Task<IActionResult> GetSummary([FromQuery] string paymentId)
  {
    var summary = await _paymentService.GetSummaryAsync(paymentId);
    return Ok(summary);
  }
}

public class CreatePreferenceDto
{
  public string Title { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}

public class MercadoPagoWebhookDto
{
  [JsonPropertyName("action")]
  public string Action { get; set; }

  [JsonPropertyName("api_version")]
  public string ApiVersion { get; set; }

  [JsonPropertyName("data")]
  public MercadoPagoDataDto Data { get; set; }

  [JsonPropertyName("date_created")]
  public DateTime DateCreated { get; set; }

  [JsonPropertyName("id")]
  public long Id { get; set; }

  [JsonPropertyName("live_mode")]
  public bool LiveMode { get; set; }

  [JsonPropertyName("type")]
  public string Type { get; set; }

  [JsonPropertyName("user_id")]
  public string UserId { get; set; }
}

public class MercadoPagoDataDto
{
  [JsonPropertyName("id")]
  public string Id { get; set; }
}