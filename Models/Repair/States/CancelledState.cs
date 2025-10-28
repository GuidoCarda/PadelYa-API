namespace padelya_api.Models.Repair.States
{
  public class CancelledState : IRepairState
  {
    public void AdvanceState(Repair repair)
    {
      // Terminal state - no further transitions
      throw new InvalidOperationException("Repair is cancelled. No further state changes allowed.");
    }

    public void CancelRepair(Repair repair)
    {
      throw new InvalidOperationException($"Cannot cancel repair in {GetStatusName()} state");
    }

    public void NotifyCustomer(Racket racket)
    {
      Console.WriteLine($"Customer notified: Repair for racket {racket.Brand} has been cancelled");
    }

    public string GetStatusName()
    {
      return "Cancelled";
    }
  }
}