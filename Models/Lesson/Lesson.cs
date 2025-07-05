using padelya_api.Models;
using padelya_api.Models.Class;

namespace padelya_api.models
{
    public class Lesson
    {
        public int Id { get; set; }
        public decimal Price { get; set; }


        // Navigation property for Teacher (one teacher per class)
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        // Navigation property for Players (1 to 4 players per class)
        public List<Player> Players { get; set; }

        // Navigation property for Stats (1 to 4 reports per class)
        public List<Stats> Reports { get; set; }

        // Navigation property for Booking (one booking per class)
        // public int BookingId { get; set; }
        // public Booking.Booking Booking { get; set; }
        // Add methods here as needed
    }
}