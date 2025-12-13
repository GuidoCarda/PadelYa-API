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
        Success = $"{_configuration["AppSettings:FrontBaseUrl"]}/bookings/success",
        Failure = $"{_configuration["AppSettings:FrontBaseUrl"]}/bookings/failure",
        Pending = $"{_configuration["AppSettings:FrontBaseUrl"]}/bookings/pending"
      },
      // {
      //   Success = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/success",
      //   Failure = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/failure",
      //   Pending = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/pending"
      // },
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
  [Microsoft.AspNetCore.Authorization.AllowAnonymous]
  public async Task<IActionResult> MercadoPagoWebhook()
  {
    Console.WriteLine("Webhook request received");
    Console.WriteLine("Webhook request received");
    Console.WriteLine("Webhook request received");
    Console.WriteLine("Webhook request received");
    try
    {
      using var reader = new StreamReader(Request.Body);
      var body = await reader.ReadToEndAsync();
      Console.WriteLine($"[Webhook Body]: {body}");

      var webhookData = JsonSerializer.Deserialize<MercadoPagoWebhookDto>(
          body,
          new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
      );

      Console.WriteLine($"webhookData: {JsonSerializer.Serialize(webhookData)}");
      if (webhookData == null)
      {
           Console.WriteLine("Webhook nulo.");
           return BadRequest("Webhook nulo.");
      }

      var hasDataId = !string.IsNullOrEmpty(webhookData.Data?.Id);
      var hasResourceId = !string.IsNullOrEmpty(webhookData.Resource);

      if (!hasDataId && !hasResourceId)
      {
        Console.WriteLine($"Webhook mal formado o faltan datos (Data.Id o Resource). Data: {JsonSerializer.Serialize(webhookData)}");
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
  [HttpPost("verify")]
  public async Task<IActionResult> VerifyPayment([FromQuery] long payment_id)
  {
      try
      {
          Console.WriteLine($"Verifying payment: {payment_id}");
          var status = await _paymentService.ProcessPaymentByIdAsync(payment_id);
          return Ok(new { status = status.ToString() });
      }
      catch (Exception ex)
      {
          Console.WriteLine($"Error verifying payment: {ex.Message}");
          return BadRequest(new { error = ex.Message });
      }
  }
}

public class CreatePreferenceDto
{
  public string Title { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}

