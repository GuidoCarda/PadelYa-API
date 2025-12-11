namespace padelya_api.Services.Email.Templates.Repair;

/// <summary>
/// Datos para el template de reparación lista para retirar.
/// </summary>
public record RepairReadyForPickupData(
    string UserName,
    string RacketBrand,
    string RacketModel,
    string RepairCode,
    string DamageDescription,
    decimal Price,
    string? RepairNotes
);

/// <summary>
/// Template de email para notificar que una reparación está lista para retirar.
/// </summary>
public class RepairReadyForPickupTemplate : IEmailTemplate<RepairReadyForPickupData>
{
  public string GetSubject(RepairReadyForPickupData data)
      => $"🎾 Tu paleta está lista para retirar - {data.RacketBrand} {data.RacketModel}";

  public string GetHtmlBody(RepairReadyForPickupData data)
  {
    var content = $"""
            <h2>Hola {data.UserName},</h2>
            <p>¡Buenas noticias! Tu paleta está <strong style="color: #22c55e;">lista para retirar</strong>.</p>
            
            <table class="info-table">
                <tr>
                    <td>Código de reparación</td>
                    <td>{data.RepairCode}</td>
                </tr>
                <tr>
                    <td>Paleta</td>
                    <td>{data.RacketBrand} {data.RacketModel}</td>
                </tr>
                <tr>
                    <td>Descripción del daño</td>
                    <td>{data.DamageDescription}</td>
                </tr>
                <tr>
                    <td>Precio</td>
                    <td>${data.Price:N2}</td>
                </tr>
                {(string.IsNullOrEmpty(data.RepairNotes) ? "" : $"<tr><td>Notas de reparación</td><td>{data.RepairNotes}</td></tr>")}
            </table>
            
            <p>Puedes pasar a retirarla en nuestro horario de atención. ¡Te esperamos!</p>
            """;

    return BaseEmailTemplate.WrapInLayout(content, "Paleta Lista para Retirar");
  }

  public string GetPlainTextBody(RepairReadyForPickupData data)
  {
    return $"""
            Hola {data.UserName},

            ¡Buenas noticias! Tu paleta está lista para retirar.

            Código de reparación: {data.RepairCode}
            Paleta: {data.RacketBrand} {data.RacketModel}
            Descripción del daño: {data.DamageDescription}
            Precio: ${data.Price:N2}
            {(string.IsNullOrEmpty(data.RepairNotes) ? "" : $"Notas de reparación: {data.RepairNotes}")}

            Puedes pasar a retirarla en nuestro horario de atención. ¡Te esperamos!

            Saludos,
            El equipo de PadelYa
            """;
  }
}



