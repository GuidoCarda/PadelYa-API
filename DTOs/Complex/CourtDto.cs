using padelya_api.Constants;

namespace padelya_api.DTOs.Complex
{
  public class CourtDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } // Ej: "Cristal", "Césped", etc.
    public CourtStatus CourtStatus { get; set; }
    public int BookingPrice { get; set; }
    public int ComplexId { get; set; }
  }
}