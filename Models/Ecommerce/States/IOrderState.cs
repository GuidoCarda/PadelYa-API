using padelya_api.Models.Ecommerce;

namespace padelya_api.Models.Ecommerce.States
{
    public interface IOrderState
    {
        void AdvanceState(Order order);
        string GetStatusName();
    }
}
