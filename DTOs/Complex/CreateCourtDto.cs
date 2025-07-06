using padelya_api.Constants;

namespace padelya_api.DTOs.Court
{
    public class CreateCourtDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public CourtStatus CourtStatus { get; set; } = CourtStatus.Available;
        public int BookingPrice { get; set; }
        public int ComplexId { get; set; } = 1;
        public TimeOnly OpeningTime { get; set; } = new TimeOnly(8, 0); // Default 8:00 AM
        public TimeOnly ClosingTime { get; set; } = new TimeOnly(23, 0); // Default 11:00 PM
    }
}