namespace padelya_api.Services.Email.Templates.Booking;

/// <summary>
/// Datos para el template de confirmación de reserva.
/// </summary>
public record BookingConfirmationData(
    string UserName,
    string CourtName,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string ComplexName,
    string? ComplexAddress,
    decimal TotalPrice,
    string BookingCode
);

/// <summary>
/// Template de email para confirmación de reserva de cancha.
/// </summary>
public class BookingConfirmationTemplate : IEmailTemplate<BookingConfirmationData>
{
  public string GetSubject(BookingConfirmationData data)
      => $"🎾 Reserva confirmada para el {data.Date:dd/MM/yyyy}";

  public string GetHtmlBody(BookingConfirmationData data)
  {
    var content = $"""
            <h2>¡Hola {data.UserName}!</h2>
            <p>Tu reserva ha sido <strong style="color: #10b981;">confirmada</strong> exitosamente.</p>
            
            <div class="highlight-box">
                <p><strong>Código de reserva:</strong></p>
                <p class="code">{data.BookingCode}</p>
            </div>
            
            <h3>📋 Detalles de la reserva</h3>
            <table class="info-table">
                <tr>
                    <td>Cancha</td>
                    <td>{data.CourtName}</td>
                </tr>
                <tr>
                    <td>Complejo</td>
                    <td>{data.ComplexName}</td>
                </tr>
                {(string.IsNullOrEmpty(data.ComplexAddress) ? "" : $"<tr><td>Dirección</td><td>{data.ComplexAddress}</td></tr>")}
                <tr>
                    <td>Fecha</td>
                    <td>{data.Date:dddd, dd 'de' MMMM 'de' yyyy}</td>
                </tr>
                <tr>
                    <td>Horario</td>
                    <td>{data.StartTime.ToString(@"hh\:mm")} - {data.EndTime.ToString(@"hh\:mm")}</td>
                </tr>
                <tr>
                    <td>Total</td>
                    <td><strong>${data.TotalPrice:N2}</strong></td>
                </tr>
            </table>
            
            <p style="color: #6b7280; font-size: 14px;">
                💡 Te recomendamos llegar 10 minutos antes de tu turno.
            </p>
            """;

    return BaseEmailTemplate.WrapInLayout(content, "Confirmación de Reserva");
  }

  public string GetPlainTextBody(BookingConfirmationData data)
  {
    return $"""
            ¡Hola {data.UserName}!

            Tu reserva ha sido confirmada exitosamente.

            Código de reserva: {data.BookingCode}

            DETALLES DE LA RESERVA:
            - Cancha: {data.CourtName}
            - Complejo: {data.ComplexName}
            {(string.IsNullOrEmpty(data.ComplexAddress) ? "" : $"- Dirección: {data.ComplexAddress}")}
            - Fecha: {data.Date:dd/MM/yyyy}
            - Horario: {data.StartTime.ToString(@"hh\:mm")} - {data.EndTime.ToString(@"hh\:mm")}
            - Total: ${data.TotalPrice:N2}

            Te recomendamos llegar 10 minutos antes de tu turno.

            Saludos,
            El equipo de PadelYa
            """;
  }
}

