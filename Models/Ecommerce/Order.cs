using System;
using System.Collections.Generic;
using padelya_api.Models;

namespace padelya_api.Models.Ecommerce
{
    public enum OrderStatus
    {
        Pending,         // Pendiente de pago
        Paid,            // Pagado
        Processing,      // En proceso
        Shipped,         // Enviado
        Delivered,       // Entregado
        Cancelled        // Cancelado
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int? PaymentId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Payment? Payment { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}

