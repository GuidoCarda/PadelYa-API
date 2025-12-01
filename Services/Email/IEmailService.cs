namespace padelya_api.Services.Email;

/// <summary>
/// Servicio base para envío de emails.
/// </summary>
public interface IEmailService
{
  /// <summary>
  /// Envía un email de forma asíncrona.
  /// </summary>
  /// <param name="to">Dirección de email del destinatario</param>
  /// <param name="subject">Asunto del email</param>
  /// <param name="htmlBody">Contenido HTML del email</param>
  /// <param name="plainTextBody">Contenido de texto plano (fallback)</param>
  Task SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null);

  /// <summary>
  /// Envía un email usando una plantilla predefinida.
  /// </summary>
  /// <typeparam name="T">Tipo de datos para la plantilla</typeparam>
  /// <param name="to">Dirección de email del destinatario</param>
  /// <param name="template">Plantilla a utilizar</param>
  /// <param name="data">Datos para renderizar la plantilla</param>
  Task SendTemplatedEmailAsync<T>(string to, IEmailTemplate<T> template, T data) where T : class;
}

