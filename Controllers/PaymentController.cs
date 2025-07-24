using Microsoft.AspNetCore.Mvc;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
  private readonly IConfiguration _configuration;

  public PaymentsController(IConfiguration configuration)
  {
    _configuration = configuration;
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
      // AutoReturn = "approved"
    };

    //  BackUrls = new PreferenceBackUrlsRequest
    //       {
    //         Success = "http://localhost:3000/bookings/pago-exitoso",
    //         Failure = "http://localhost:3000/bookings/pago-fallido",
    //         Pending = "http://localhost:3000/bookings/pago-pendiente"
    //       },
    var client = new PreferenceClient();
    Preference preference = await client.CreateAsync(request);

    return Ok(new { init_point = preference.InitPoint });
  }

  [HttpPost("webhook")]
  public async Task<IActionResult> MercadoPagoWebhook()
  {
    // Lee el body y procesa la notificación
    using var reader = new StreamReader(Request.Body);
    var body = await reader.ReadToEndAsync();

    Console.WriteLine(body);

    // Aquí deberías validar y actualizar el estado del pago en tu base de datos

    return Ok();
  }

}

// DTO para recibir los datos
public class CreatePreferenceDto
{
  public string Title { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}