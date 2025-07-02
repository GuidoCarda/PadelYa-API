using padelya_api.Constants;

namespace padelya_api.Models
{
    public class CourtAvailability
    {
        public int Id { get; set; }
        public Weekday Weekday { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int CourtId { get; set; }
        public Court Court { get; set; }
    }
}