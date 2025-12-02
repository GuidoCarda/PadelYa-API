namespace padelya_api.Services.Email.Templates.Booking;

/// <summary>
/// Datos para el template de cancelación de reserva.
/// </summary>
public record BookingCancellationData(
    string UserName,
    string CourtName,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string BookingCode,
    string? CancellationReason
);

/// <summary>
/// Template de email para cancelación de reserva.
/// </summary>
public class BookingCancellationTemplate : IEmailTemplate<BookingCancellationData>
{
  public string GetSubject(BookingCancellationData data)
      => $"❌ Reserva cancelada - {data.Date:dd/MM/yyyy}";

  public string GetHtmlBody(BookingCancellationData data)
  {
    var content = $"""
            <h2>Hola {data.UserName},</h2>
            <p>Tu reserva ha sido <strong style="color: #ef4444;">cancelada</strong>.</p>
            
            <table class="info-table">
                <tr>
                    <td>Código de reserva</td>
                    <td>{data.BookingCode}</td>
                </tr>
                <tr>
                    <td>Cancha</td>
                    <td>{data.CourtName}</td>
                </tr>
                <tr>
                    <td>Fecha</td>
                    <td>{data.Date:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <td>Horario</td>
                    <td>{data.StartTime.ToString(@"hh\:mm")} - {data.EndTime.ToString(@"hh\:mm")}</td>
                </tr>
                {(string.IsNullOrEmpty(data.CancellationReason) ? "" : $"<tr><td>Motivo</td><td>{data.CancellationReason}</td></tr>")}
            </table>
            
            <p>Si esto fue un error o necesitas hacer una nueva reserva, puedes hacerlo desde la app.</p>
            """;

    return BaseEmailTemplate.WrapInLayout(content, "Reserva Cancelada");
  }

  public string GetPlainTextBody(BookingCancellationData data)
  {
    return $"""
            Hola {data.UserName},

            Tu reserva ha sido cancelada.

            Código de reserva: {data.BookingCode}
            Cancha: {data.CourtName}
            Fecha: {data.Date:dd/MM/yyyy}
            Horario: {data.StartTime.ToString(@"hh\:mm")} - {data.EndTime.ToString(@"hh\:mm")}
            {(string.IsNullOrEmpty(data.CancellationReason) ? "" : $"Motivo: {data.CancellationReason}")}

            Si esto fue un error o necesitas hacer una nueva reserva, puedes hacerlo desde la app.

            Saludos,
            El equipo de PadelYa
            """;
  }
}

