namespace padelya_api.Constants
{
    public enum OrderStatus
    {
        Draft = -1,      // Borrador (invisible al usuario)
        Pending = 0,     // Pendiente de pago
        Paid,            // Pagado
        Progress,        // En proceso (Preparation)
        PickUp,          // Listo para retirar
        Success,         // Entregado/Completado
        Cancelled,       // Cancelado
        Processing,      // Deprecated or keep for compatibility
        Shipped,         // Deprecated or keep for compatibility
        Delivered        // Deprecated or keep for compatibility
    }
}
