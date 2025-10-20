namespace padelya_api.Models.Repair.States
{
  public enum RepairStatus
  {
    Received,
    InRepair,
    ReadyForPickup,
    Delivered,
    Cancelled
  }

  public interface IRepairState
  {
    void AdvanceState(Repair repair);
    void CancelRepair(Repair repair);
    void NotifyCustomer(Racket racket);
    string GetStatusName();
  }
}