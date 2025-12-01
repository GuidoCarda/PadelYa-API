namespace padelya_api.Services.Email.Templates.Ecommerce;

/// <summary>
/// Representa un item de la compra.
/// </summary>
public record PurchaseItem(
    string Name,
    int Quantity,
    decimal UnitPrice
);

/// <summary>
/// Datos para el template de confirmación de compra.
/// </summary>
public record PurchaseConfirmationData(
    string UserName,
    string OrderNumber,
    DateTime PurchaseDate,
    List<PurchaseItem> Items,
    decimal Subtotal,
    decimal Tax,
    decimal Total,
    string? DeliveryAddress
);

/// <summary>
/// Template de email para confirmación de compra en ecommerce.
/// </summary>
public class PurchaseConfirmationTemplate : IEmailTemplate<PurchaseConfirmationData>
{
    public string GetSubject(PurchaseConfirmationData data)
        => $"🛒 Compra confirmada - Orden #{data.OrderNumber}";

    public string GetHtmlBody(PurchaseConfirmationData data)
    {
        var itemsHtml = string.Join("", data.Items.Select(item => $"""
            <tr>
                <td style="padding: 10px; border-bottom: 1px solid #e5e7eb;">{item.Name}</td>
                <td style="padding: 10px; border-bottom: 1px solid #e5e7eb; text-align: center;">{item.Quantity}</td>
                <td style="padding: 10px; border-bottom: 1px solid #e5e7eb; text-align: right;">${item.UnitPrice:N2}</td>
                <td style="padding: 10px; border-bottom: 1px solid #e5e7eb; text-align: right;">${(item.Quantity * item.UnitPrice):N2}</td>
            </tr>
            """));

        var content = $"""
            <h2>¡Gracias por tu compra, {data.UserName}!</h2>
            <p>Hemos recibido tu orden y está siendo procesada.</p>
            
            <div class="highlight-box">
                <p><strong>Número de orden:</strong></p>
                <p class="code">{data.OrderNumber}</p>
                <p style="margin: 0; font-size: 14px; color: #6b7280;">Fecha: {data.PurchaseDate:dd/MM/yyyy HH:mm}</p>
            </div>
            
            <h3>📦 Detalle de tu compra</h3>
            <table style="width: 100%; border-collapse: collapse; margin: 20px 0;">
                <thead>
                    <tr style="background: #f9fafb;">
                        <th style="padding: 10px; text-align: left; border-bottom: 2px solid #e5e7eb;">Producto</th>
                        <th style="padding: 10px; text-align: center; border-bottom: 2px solid #e5e7eb;">Cant.</th>
                        <th style="padding: 10px; text-align: right; border-bottom: 2px solid #e5e7eb;">Precio</th>
                        <th style="padding: 10px; text-align: right; border-bottom: 2px solid #e5e7eb;">Total</th>
                    </tr>
                </thead>
                <tbody>
                    {itemsHtml}
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan="3" style="padding: 10px; text-align: right;">Subtotal</td>
                        <td style="padding: 10px; text-align: right;">${data.Subtotal:N2}</td>
                    </tr>
                    <tr>
                        <td colspan="3" style="padding: 10px; text-align: right;">IVA</td>
                        <td style="padding: 10px; text-align: right;">${data.Tax:N2}</td>
                    </tr>
                    <tr style="font-weight: bold; font-size: 18px;">
                        <td colspan="3" style="padding: 10px; text-align: right;">Total</td>
                        <td style="padding: 10px; text-align: right; color: #2563eb;">${data.Total:N2}</td>
                    </tr>
                </tfoot>
            </table>
            
            {(string.IsNullOrEmpty(data.DeliveryAddress) ? "" : $"""
            <h3>📍 Dirección de entrega</h3>
            <p style="background: #f9fafb; padding: 15px; border-radius: 6px;">{data.DeliveryAddress}</p>
            """)}
            
            <p>Te notificaremos cuando tu pedido esté listo o en camino.</p>
            """;

        return BaseEmailTemplate.WrapInLayout(content, "Confirmación de Compra");
    }

    public string GetPlainTextBody(PurchaseConfirmationData data)
    {
        var itemsText = string.Join("\n", data.Items.Select(item =>
            $"  - {item.Name} x{item.Quantity} = ${(item.Quantity * item.UnitPrice):N2}"));

        return $"""
            ¡Gracias por tu compra, {data.UserName}!

            Hemos recibido tu orden y está siendo procesada.

            Número de orden: {data.OrderNumber}
            Fecha: {data.PurchaseDate:dd/MM/yyyy HH:mm}

            DETALLE DE TU COMPRA:
            {itemsText}

            Subtotal: ${data.Subtotal:N2}
            IVA: ${data.Tax:N2}
            Total: ${data.Total:N2}

            {(string.IsNullOrEmpty(data.DeliveryAddress) ? "" : $"Dirección de entrega: {data.DeliveryAddress}")}

            Te notificaremos cuando tu pedido esté listo o en camino.

            Saludos,
            El equipo de PadelYa
            """;
    }
}

