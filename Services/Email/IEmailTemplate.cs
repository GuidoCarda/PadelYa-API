namespace padelya_api.Services.Email;

/// <summary>
/// Interfaz base para templates de email tipados.
/// Cada módulo puede definir sus propios templates implementando esta interfaz.
/// </summary>
/// <typeparam name="T">Tipo de datos que recibe el template</typeparam>
public interface IEmailTemplate<T> where T : class
{
  /// <summary>
  /// Asunto del email
  /// </summary>
  string GetSubject(T data);

  /// <summary>
  /// Contenido HTML del email
  /// </summary>
  string GetHtmlBody(T data);

  /// <summary>
  /// Contenido de texto plano (opcional, para clientes sin soporte HTML)
  /// </summary>
  string? GetPlainTextBody(T data) => null;
}

