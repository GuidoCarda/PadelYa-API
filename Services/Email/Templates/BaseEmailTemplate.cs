namespace padelya_api.Services.Email.Templates;

/// <summary>
/// Template base con estilos comunes para todos los emails de PadelYa.
/// </summary>
public static class BaseEmailTemplate
{
  public static string WrapInLayout(string content, string title)
  {
    return $$"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>{{title}}</title>
            <style>
                body {
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    margin: 0;
                    padding: 0;
                    background-color: #f4f4f4;
                }
                .container {
                    max-width: 600px;
                    margin: 20px auto;
                    background: #ffffff;
                    border-radius: 8px;
                    overflow: hidden;
                    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                }
                .header {
                    background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
                    color: white;
                    padding: 30px;
                    text-align: center;
                }
                .header h1 {
                    margin: 0;
                    font-size: 28px;
                    font-weight: 600;
                }
                .content {
                    padding: 30px;
                }
                .button {
                    display: inline-block;
                    background: #2563eb;
                    color: white;
                    padding: 12px 30px;
                    text-decoration: none;
                    border-radius: 6px;
                    font-weight: 600;
                    margin: 20px 0;
                }
                .button:hover {
                    background: #1d4ed8;
                }
                .highlight-box {
                    background: #f0f9ff;
                    border-left: 4px solid #2563eb;
                    padding: 15px;
                    margin: 20px 0;
                    border-radius: 0 6px 6px 0;
                }
                .code {
                    font-family: 'Courier New', monospace;
                    background: #e5e7eb;
                    padding: 10px 20px;
                    border-radius: 6px;
                    font-size: 18px;
                    letter-spacing: 2px;
                    display: inline-block;
                }
                .footer {
                    background: #f9fafb;
                    padding: 20px;
                    text-align: center;
                    font-size: 12px;
                    color: #6b7280;
                    border-top: 1px solid #e5e7eb;
                }
                .info-table {
                    width: 100%;
                    border-collapse: collapse;
                    margin: 20px 0;
                }
                .info-table td {
                    padding: 10px;
                    border-bottom: 1px solid #e5e7eb;
                }
                .info-table td:first-child {
                    font-weight: 600;
                    color: #6b7280;
                    width: 40%;
                }
            </style>
        </head>
        <body>
            <div class="container">
                <div class="header">
                    <h1>🎾 PadelYa</h1>
                </div>
                <div class="content">
                    {{content}}
                </div>
                <div class="footer">
                    <p>Este es un email automático, por favor no respondas a este mensaje.</p>
                    <p>© {{DateTime.Now.Year}} PadelYa - Todos los derechos reservados</p>
                </div>
            </div>
        </body>
        </html>
        """;
  }
}
