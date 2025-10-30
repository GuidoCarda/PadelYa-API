namespace padelya_api.Models.Repair.States
{
  public class ReadyForPickupState : IRepairState
  {
    public void AdvanceState(Repair repair)
    {
      repair.State = new DeliveredState();
      repair.Status = RepairStatus.Delivered;
      repair.DeliveredAt = DateTime.Now;
    }

    public void CancelRepair(Repair repair)
    {
      throw new InvalidOperationException($"Cannot cancel repair in {GetStatusName()} state");
    }

    public void NotifyCustomer(Racket racket)
    {
      Console.WriteLine($"Customer notified: Racket {racket.Brand} is ready for pickup!");
    }

    public string GetStatusName()
    {
      return "Ready for Pickup";
    }
  }
}