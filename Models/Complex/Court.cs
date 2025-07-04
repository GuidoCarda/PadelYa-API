using padelya_api.Constants;

namespace padelya_api.Models
{
    public class Court
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CourtStatus CourtStatus { get; set; }
        public int BookingPrice { get; set; }

        public int ComplexId { get; set; }

        public List<CourtAvailability> Availability { get; set; } = [];
    }
}