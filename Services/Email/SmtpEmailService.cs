using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace padelya_api.Services.Email;

/// <summary>
/// Implementación del servicio de email usando SMTP.
/// </summary>
public class SmtpEmailService : IEmailService
{
  private readonly SmtpSettings _settings;
  private readonly ILogger<SmtpEmailService> _logger;

  public SmtpEmailService(IOptions<SmtpSettings> settings, ILogger<SmtpEmailService> logger)
  {
    _settings = settings.Value;
    _logger = logger;
  }

  public async Task SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null)
  {
    try
    {
      using var client = new SmtpClient(_settings.Host, _settings.Port)
      {
        EnableSsl = _settings.EnableSsl,
        Credentials = new NetworkCredential(_settings.Username, _settings.Password)
      };

      var message = new MailMessage
      {
        From = new MailAddress(_settings.FromEmail, _settings.FromName),
        Subject = subject,
        IsBodyHtml = true,
        Body = htmlBody
      };

      message.To.Add(to);

      // Agregar versión de texto plano si está disponible
      if (!string.IsNullOrEmpty(plainTextBody))
      {
        var plainView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, "text/plain");
        var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
        message.AlternateViews.Add(plainView);
        message.AlternateViews.Add(htmlView);
      }

      await client.SendMailAsync(message);
      _logger.LogInformation("Email enviado exitosamente a {To} con asunto: {Subject}", to, subject);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al enviar email a {To} con asunto: {Subject}", to, subject);
      throw;
    }
  }

  public async Task SendTemplatedEmailAsync<T>(string to, IEmailTemplate<T> template, T data) where T : class
  {
    var subject = template.GetSubject(data);
    var htmlBody = template.GetHtmlBody(data);
    var plainTextBody = template.GetPlainTextBody(data);


    Console.WriteLine($"Sending email to {to} with subject: {subject}");
    Console.WriteLine($"HTML Body: {htmlBody}");
    Console.WriteLine($"Plain Text Body: {plainTextBody}");
    await SendEmailAsync(to, subject, htmlBody, plainTextBody);
  }
}

/// <summary>
/// Configuración SMTP para el envío de emails.
/// </summary>
public class SmtpSettings
{
  public string Host { get; set; } = string.Empty;
  public int Port { get; set; } = 587;
  public bool EnableSsl { get; set; } = true;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string FromEmail { get; set; } = string.Empty;
  public string FromName { get; set; } = "PadelYa";
}

