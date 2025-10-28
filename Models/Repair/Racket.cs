namespace padelya_api.Models.Repair
{
  public class Racket
  {
    public int Id { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public required string SerialCode { get; set; }
  }
}