using padelya_api.Constants;
using padelya_api.Models;

namespace padelya_api.DTOs.Court
{
    public class CreateCourtDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public CourtStatus CourtStatus { get; set; } = CourtStatus.Available;
        public int BookingPrice { get; set; }
        public int ComplexId { get; set; } = 1;
        public List<CourtAvailability> availability { get; set; } = [];
    }
}