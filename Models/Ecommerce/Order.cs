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
            _currentState = GetStateFromStatus(Status);
        }

        public void SetState(IOrderState state)
        {
            _currentState = state;
        }

        // Automatic progression to next state
        public void AdvanceState()
        {
            // Sync state before advancing in case it was loaded from DB
            if (_currentState.GetStatusName() != GetStateFromStatus(Status).GetStatusName())
            {
               _currentState = GetStateFromStatus(Status);
            }
            
            _currentState.AdvanceState(this);
        }

        // State-controlled transition methods (Pure State Pattern)
        public void MarkAsPaid()
        {
            _currentState.MarkAsPaid(this);
        }

        public void StartProcessing()
        {
            _currentState.StartProcessing(this);
        }

        public void MarkAsReadyForPickup()
        {
            _currentState.MarkAsReadyForPickup(this);
        }

        public void Complete()
        {
            _currentState.Complete(this);
        }

        public void Cancel()
        {
            _currentState.Cancel(this);
        }

        public string GetStatusName()
        {
             if (_currentState == null) _currentState = GetStateFromStatus(Status);
             return _currentState.GetStatusName();
        }

        public bool CanTransitionTo(string targetState)
        {
            if (_currentState == null) _currentState = GetStateFromStatus(Status);
            return _currentState.CanTransitionTo(targetState);
        }

        // Initialize state from DB status (for EF Core)
        public void InitializeState()
        {
            _currentState = GetStateFromStatus(Status);
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
