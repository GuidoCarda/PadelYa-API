using padelya_api.Models.Ecommerce;

namespace padelya_api.Models.Ecommerce.States
{
    public interface IOrderState
    {
        // Progresión automática al siguiente estado
        void AdvanceState(Order order);
        
        //cada estado decide si es válido.
        void MarkAsPaid(Order order);
        void StartProcessing(Order order);
        void MarkAsReadyForPickup(Order order);
        void Complete(Order order);
        void Cancel(Order order);
        
        string GetStatusName();
        
        // comprobar si una transición es válida
        bool CanTransitionTo(string targetState);
    }
}
