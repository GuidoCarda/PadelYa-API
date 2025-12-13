using padelya_api.Constants;

namespace padelya_api.Models.Ecommerce.States
{
    public class PendingState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Pending -> Progress (Paid)
            order.Status = OrderStatus.Progress;
            order.SetState(new ProgressState());
        }

        public string GetStatusName() => "Pendiente";
    }

    public class ProgressState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Progress -> PickUp
            order.Status = OrderStatus.PickUp;
            order.SetState(new PickUpState());
        }

        public string GetStatusName() => "En Progreso";
    }

    public class PickUpState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // PickUp -> Success (Delivered/Picked up)
            order.Status = OrderStatus.Success;
            order.SetState(new SuccessState());
        }

        public string GetStatusName() => "Listo para retirar";
    }

    public class SuccessState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Final state, no transition
            // Could throw exception or do nothing
        }

        public string GetStatusName() => "Completado";
    }

    public class CancelledState : IOrderState
    {
        public void AdvanceState(Order order)
        {
            // Final state
        }

        public string GetStatusName() => "Cancelado";
    }
}
