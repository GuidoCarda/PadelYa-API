using padelya_api.Constants;

namespace padelya_api.Models
{
    public class Court
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CourtStatus CourtStatus { get; set; }
        public int BookingPrice { get; set; }

        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }

        public int ComplexId { get; set; }

        public string Type { get; set; } // Ej: "Cristal", "Césped", etc.
    }
}