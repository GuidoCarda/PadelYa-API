namespace padelya_api.Models.Repair.States
{
  public class ReceivedState : IRepairState
  {
    public void AdvanceState(Repair repair)
    {
      repair.State = new InRepairState();
      repair.Status = RepairStatus.InRepair;
    }
    public void CancelRepair(Repair repair)
    {
      repair.State = new CancelledState();
      repair.Status = RepairStatus.Cancelled;
    }

    public void NotifyCustomer(Racket racket)
    {
      // Send email/SMS: "We received your racket [brand] [serial]"
      Console.WriteLine($"Customer notified: Racket {racket.Brand} received");
    }

    public string GetStatusName()
    {
      return "Received";
    }
  }
}