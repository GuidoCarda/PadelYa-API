namespace padelya_api.Services.Email.Templates.Auth;

/// <summary>
/// Datos para el template de contraseña de respaldo.
/// </summary>
public record BackupPasswordData(
    string UserName,
    string BackupPassword,
    DateTime ExpirationDate
);

/// <summary>
/// Template de email para envío de contraseña de respaldo.
/// </summary>
public class BackupPasswordTemplate : IEmailTemplate<BackupPasswordData>
{
  public string GetSubject(BackupPasswordData data)
      => "🔑 Tu contraseña de respaldo de PadelYa";

  public string GetHtmlBody(BackupPasswordData data)
  {
    var content = $"""
            <h2>Hola {data.UserName},</h2>
            <p>Se ha generado una contraseña de respaldo para tu cuenta.</p>
            
            <div class="highlight-box">
                <p><strong>Tu contraseña de respaldo es:</strong></p>
                <p class="code">{data.BackupPassword}</p>
            </div>
            
            <p><strong>Importante:</strong></p>
            <ul>
                <li>Guarda esta contraseña en un lugar seguro</li>
                <li>No la compartas con nadie</li>
                <li>Expira el: <strong>{data.ExpirationDate:dd/MM/yyyy HH:mm}</strong></li>
            </ul>
            
            <p>Puedes usar esta contraseña para acceder a tu cuenta si olvidas tu contraseña principal.</p>
            """;

    return BaseEmailTemplate.WrapInLayout(content, "Contraseña de Respaldo");
  }

  public string GetPlainTextBody(BackupPasswordData data)
  {
    return $"""
            Hola {data.UserName},

            Se ha generado una contraseña de respaldo para tu cuenta.

            Tu contraseña de respaldo es: {data.BackupPassword}

            Importante:
            - Guarda esta contraseña en un lugar seguro
            - No la compartas con nadie
            - Expira el: {data.ExpirationDate:dd/MM/yyyy HH:mm}

            Puedes usar esta contraseña para acceder a tu cuenta si olvidas tu contraseña principal.

            Saludos,
            El equipo de PadelYa
            """;
  }
}

