namespace padelya_api.Models.Repair.States
{
  public class DeliveredState : IRepairState
  {
    public void AdvanceState(Repair repair)
    {
      // Terminal state - no further transitions
      throw new InvalidOperationException("Repair is already delivered. No further state changes allowed.");
    }

    public void CancelRepair(Repair repair)
    {
      throw new InvalidOperationException($"Cannot cancel repair in {GetStatusName()} state");
    }

    public void NotifyCustomer(Racket racket)
    {
      Console.WriteLine($"Customer notified: Thank you! Racket {racket.Brand} delivered");
    }

    public string GetStatusName()
    {
      return "Delivered";
    }
  }
}