using System.Text.Json.Serialization;
using padelya_api.Constants;

namespace padelya_api.Models
{
    public class CourtAvailability
    {
        public int Id { get; set; }
        public Weekday Weekday { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public int CourtId { get; set; }
    }
}