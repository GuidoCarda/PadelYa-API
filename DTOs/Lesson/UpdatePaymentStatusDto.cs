namespace padelya_api.DTOs.Lesson
{
    public class UpdatePaymentStatusDto
    {
        public string PaymentStatus { get; set; } = string.Empty; // Pagado, Pendiente, Rechazado
    }
}

