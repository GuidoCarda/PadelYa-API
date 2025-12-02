namespace padelya_api.Services.Email.Templates.Auth;

/// <summary>
/// Datos para el template de recuperación de contraseña.
/// </summary>
public record PasswordRecoveryData(
    string UserName,
    string NewPassword
);

/// <summary>
/// Template de email para recuperación de contraseña.
/// </summary>
public class PasswordRecoveryTemplate : IEmailTemplate<PasswordRecoveryData>
{
  public string GetSubject(PasswordRecoveryData data)
      => "🔐 Tu nueva contraseña de PadelYa";

  public string GetHtmlBody(PasswordRecoveryData data)
  {
    var content = $"""
            <h2>Hola {data.UserName},</h2>
            <p>Recibimos una solicitud para restablecer tu contraseña.</p>
            
            <div class="highlight-box">
                <p><strong>Tu nueva contraseña temporal es:</strong></p>
                <p class="code">{data.NewPassword}</p>
            </div>
            
            <p>Por seguridad, te recomendamos cambiar esta contraseña después de iniciar sesión.</p>
            
            <p style="color: #ef4444; font-size: 14px;">
                ⚠️ Si no solicitaste este cambio, contacta con nosotros inmediatamente.
            </p>
            """;

    return BaseEmailTemplate.WrapInLayout(content, "Recuperación de Contraseña");
  }

  public string GetPlainTextBody(PasswordRecoveryData data)
  {
    return $"""
            Hola {data.UserName},

            Recibimos una solicitud para restablecer tu contraseña.

            Tu nueva contraseña temporal es: {data.NewPassword}

            Por seguridad, te recomendamos cambiar esta contraseña después de iniciar sesión.

            Si no solicitaste este cambio, contacta con nosotros inmediatamente.

            Saludos,
            El equipo de PadelYa
            """;
  }
}

