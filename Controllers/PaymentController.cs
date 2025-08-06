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
      BackUrls = new PreferenceBackUrlsRequest
      {
        Success = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/success",
        Failure = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/failure",
        Pending = "https://9pkvr4lt-3000.brs.devtunnels.ms/bookings/pending"
      },
      AutoReturn = "approved"
    };

    var client = new PreferenceClient();
    Preference preference = await client.CreateAsync(request);

    return Ok(new { init_point = preference.InitPoint });
  }

  [HttpPost("webhook")]
  public async Task<IActionResult> MercadoPagoWebhook()
  {
    try
    {
      using var reader = new StreamReader(Request.Body);
      var body = await reader.ReadToEndAsync();

      Console.WriteLine($"Webhook recibido: {body}");

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

      if (webhookData == null)
      {
        return BadRequest("Datos del webhook inválidos");
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