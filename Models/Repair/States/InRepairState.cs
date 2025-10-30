namespace padelya_api.Models.Repair.States
{
  public class InRepairState : IRepairState
  {
    public void AdvanceState(Repair repair)
    {
      repair.State = new ReadyForPickupState();
      repair.Status = RepairStatus.ReadyForPickup;
      repair.FinishedAt = DateTime.Now;
    }

    public void CancelRepair(Repair repair)
    {
      repair.State = new CancelledState();
      repair.Status = RepairStatus.Cancelled;
    }

    public void NotifyCustomer(Racket racket)
    {
      Console.WriteLine($"Customer notified: Racket {racket.Brand} is being repaired");
    }

    public string GetStatusName()
    {
      return "In Repair";
    }
  }
}