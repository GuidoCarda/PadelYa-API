using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using padelya_api.Models;
using padelya_api.Constants;
using padelya_api.Models.Ecommerce.States;

namespace padelya_api.Models.Ecommerce
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int PersonId { get; set; }
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // Mercado Pago Preference ID
        public string? PreferenceId { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [NotMapped]
        private IOrderState _currentState;

        public Order()
        {
            // Initialize based on Status property if needed, mostly for EF
            // Logic to sync _currentState with Status would be in a method or property setter
            _currentState = GetStateFromStatus(Status);
        }

        public void SetState(IOrderState state)
        {
            _currentState = state;
        }

        public void AdvanceState()
        {
            // Sync state before advancing in case it was loaded from DB
            if (_currentState.GetStatusName() != GetStateFromStatus(Status).GetStatusName())
            {
               _currentState = GetStateFromStatus(Status);
            }
            
            _currentState.AdvanceState(this);
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
            _currentState = GetStateFromStatus(Status);
        }

        public string GetStatusName()
        {
             if (_currentState == null) _currentState = GetStateFromStatus(Status);
             return _currentState.GetStatusName();
        }

        private IOrderState GetStateFromStatus(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => new PendingState(),
                OrderStatus.Paid => new PaidState(),
                OrderStatus.Progress => new ProgressState(),
                OrderStatus.PickUp => new PickUpState(),
                OrderStatus.Success => new SuccessState(),
                OrderStatus.Cancelled => new CancelledState(),
                _ => new PendingState()
            };
        }
    }
}
