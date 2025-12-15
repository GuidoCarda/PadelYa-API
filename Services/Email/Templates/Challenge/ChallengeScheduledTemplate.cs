using padelya_api.Services.Email.Templates.Booking;

namespace padelya_api.Services.Email.Templates.Challenge;

/// <summary>
/// Datos para el template de confirmación de desafío agendado.
/// </summary>
public record ChallengeScheduledData(
    string UserName,
    string OpponentNames,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string CourtName,
    string ChallengeCode
);

/// <summary>
/// Template de email para confirmación de desafío con fecha y hora definidos.
/// </summary>
public class ChallengeScheduledTemplate : IEmailTemplate<ChallengeScheduledData>
{
  public string GetSubject(ChallengeScheduledData data)
      => $"🎾 Desafío confirmado para el {data.Date:dd/MM/yyyy}";

  public string GetHtmlBody(ChallengeScheduledData data)
  {
    var content = $"""
            <h2>¡Hola {data.UserName}!</h2>
            <p>Tu desafío ha sido <strong style="color: #10b981;">confirmado</strong> exitosamente.</p>

            <div class="highlight-box">
                <p><strong>Código de desafío:</strong></p>
                <p class="code">{data.ChallengeCode}</p>
            </div>

            <h3>📋 Detalles del desafío</h3>
            <table class="info-table">
                <tr>
                    <td>Rivales</td>
                    <td>{data.OpponentNames}</td>
                </tr>
                <tr>
                    <td>Cancha</td>
                    <td>{data.CourtName}</td>
                </tr>
                <tr>
                    <td>Fecha</td>
                    <td>{data.Date:dddd, dd 'de' MMMM 'de' yyyy}</td>
                </tr>
                <tr>
                    <td>Horario</td>
                    <td>{data.StartTime.ToString(@"hh\:mm")} - {data.EndTime.ToString(@"hh\:mm")}</td>
                </tr>
            </table>

            <p style="color: #6b7280; font-size: 14px;">
                💡 Te recomendamos llegar 10 minutos antes del horario de inicio.
            </p>
            """;

    return BaseEmailTemplate.WrapInLayout(content, "Confirmación de Desafío");
  }

  public string GetPlainTextBody(ChallengeScheduledData data)
  {
    return $"""
            ¡Hola {data.UserName}!

            Tu desafío ha sido confirmado exitosamente.

            Código de desafío: {data.ChallengeCode}

            DETALLES DEL DESAFÍO:
            - Rivales: {data.OpponentNames}
            - Cancha: {data.CourtName}
            - Fecha: {data.Date:dd/MM/yyyy}
            - Horario: {data.StartTime.ToString(@"hh\:mm")} - {data.EndTime.ToString(@"hh\:mm")}

            Te recomendamos llegar 10 minutos antes del horario de inicio.

            Saludos,
            El equipo de PadelYa
            """;
  }
}

