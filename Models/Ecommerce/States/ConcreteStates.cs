using padelya_api.Constants;

namespace padelya_api.Models.Ecommerce.States
{
    public class PendingState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Pending -> Paid (when payment is confirmed)
            MarkAsPaid(order);
        }

        public void MarkAsPaid(Order order)
        {
            order.Status = OrderStatus.Paid;
            order.SetState(new PaidState());
            Console.WriteLine($"[STATE] Order {order.Id}: Pending -> Paid");
        }

        public void StartProcessing(Order order)
        {
            throw new InvalidOperationException("Cannot start processing a pending order. Payment must be confirmed first.");
        }

        public void MarkAsReadyForPickup(Order order)
        {
            throw new InvalidOperationException("Cannot mark as ready for pickup from pending state.");
        }

        public void Complete(Order order)
        {
            throw new InvalidOperationException("Cannot complete a pending order.");
        }

        public void Cancel(Order order)
        {
            order.Status = OrderStatus.Cancelled;
            order.SetState(new CancelledState());
            Console.WriteLine($"[STATE] Order {order.Id}: Pending -> Cancelled");
        }

        public string GetStatusName() => "Pendiente";

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "Paid" || targetState == "Cancelled";
        }
    }

    public class PaidState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Paid -> Progress (automatically start processing)
            StartProcessing(order);
        }

        public void MarkAsPaid(Order order)
        {
            // Already paid, do nothing
            Console.WriteLine($"[STATE] Order {order.Id}: Already in Paid state");
        }

        public void StartProcessing(Order order)
        {
            order.Status = OrderStatus.Progress;
            order.SetState(new ProgressState());
            Console.WriteLine($"[STATE] Order {order.Id}: Paid -> Progress");
        }

        public void MarkAsReadyForPickup(Order order)
        {
            throw new InvalidOperationException("Cannot mark as ready for pickup. Order must be in progress first.");
        }

        public void Complete(Order order)
        {
            throw new InvalidOperationException("Cannot complete from paid state. Order must go through processing.");
        }

        public void Cancel(Order order)
        {
            order.Status = OrderStatus.Cancelled;
            order.SetState(new CancelledState());
            Console.WriteLine($"[STATE] Order {order.Id}: Paid -> Cancelled");
        }

        public string GetStatusName() => "Pagado";

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "Progress" || targetState == "Cancelled";
        }
    }

    public class ProgressState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Progress -> PickUp
            MarkAsReadyForPickup(order);
        }

        public void MarkAsPaid(Order order)
        {
            // Already paid, do nothing
            Console.WriteLine($"[STATE] Order {order.Id}: Already paid (in Progress)");
        }

        public void StartProcessing(Order order)
        {
            // Already in progress
            Console.WriteLine($"[STATE] Order {order.Id}: Already in Progress state");
        }

        public void MarkAsReadyForPickup(Order order)
        {
            order.Status = OrderStatus.PickUp;
            order.SetState(new PickUpState());
            Console.WriteLine($"[STATE] Order {order.Id}: Progress -> PickUp");
        }

        public void Complete(Order order)
        {
            throw new InvalidOperationException("Cannot complete from progress state. Order must be ready for pickup first.");
        }

        public void Cancel(Order order)
        {
            order.Status = OrderStatus.Cancelled;
            order.SetState(new CancelledState());
            Console.WriteLine($"[STATE] Order {order.Id}: Progress -> Cancelled");
        }

        public string GetStatusName() => "En Progreso";

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "PickUp" || targetState == "Cancelled";
        }
    }

    public class PickUpState : IOrderState
    {
        public void AdvanceState(Order order)
        {
        
            Complete(order);
        }

        public void MarkAsPaid(Order order)
        {
            
            Console.WriteLine($"[STATE] Order {order.Id}: Already paid (in PickUp)");
        }

        public void StartProcessing(Order order)
        {
            throw new InvalidOperationException("Cannot go back to processing from pickup state.");
        }

        public void MarkAsReadyForPickup(Order order)
        {
            
            Console.WriteLine($"[STATE] Order {order.Id}: Already in PickUp state");
        }
        // ✅ Transición VÁLIDA
        public void Complete(Order order)
        {
            order.Status = OrderStatus.Success;
            order.SetState(new SuccessState());
            Console.WriteLine($"[STATE] Order {order.Id}: PickUp -> Success");
        }

        public void Cancel(Order order)
        {
            order.Status = OrderStatus.Cancelled;
            order.SetState(new CancelledState());
            Console.WriteLine($"[STATE] Order {order.Id}: PickUp -> Cancelled");
        }

        public string GetStatusName() => "Listo para retirar";

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "Success" || targetState == "Cancelled";
        }
    }

    public class SuccessState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Final state, no transition
            Console.WriteLine($"[STATE] Order {order.Id}: Already completed");
        }

        public void MarkAsPaid(Order order)
        {
            throw new InvalidOperationException("Order is already completed.");
        }

        public void StartProcessing(Order order)
        {
            throw new InvalidOperationException("Order is already completed.");
        }

        public void MarkAsReadyForPickup(Order order)
        {
            throw new InvalidOperationException("Order is already completed.");
        }

        public void Complete(Order order)
        {
            // Already completed
            Console.WriteLine($"[STATE] Order {order.Id}: Already in Success state");
        }

        public void Cancel(Order order)
        {
            throw new InvalidOperationException("Cannot cancel a completed order.");
        }

        public string GetStatusName() => "Completado";

        public bool CanTransitionTo(string targetState)
        {
            return false; // Final state
        }
    }

    public class CancelledState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Final state
            Console.WriteLine($"[STATE] Order {order.Id}: Order is cancelled, no further transitions");
        }

        public void MarkAsPaid(Order order)
        {
            throw new InvalidOperationException("Cannot mark a cancelled order as paid.");
        }

        public void StartProcessing(Order order)
        {
            throw new InvalidOperationException("Cannot process a cancelled order.");
        }

        public void MarkAsReadyForPickup(Order order)
        {
            throw new InvalidOperationException("Cannot mark a cancelled order as ready for pickup.");
        }

        public void Complete(Order order)
        {
            throw new InvalidOperationException("Cannot complete a cancelled order.");
        }

        public void Cancel(Order order)
        {
            // Already cancelled
            Console.WriteLine($"[STATE] Order {order.Id}: Already in Cancelled state");
        }

        public string GetStatusName() => "Cancelado";

        public bool CanTransitionTo(string targetState)
        {
            return false; // Final state
        }
    }
}
