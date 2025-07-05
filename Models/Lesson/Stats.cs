using padelya_api.Models;

namespace padelya_api.Models.Class
{
    public class Stats
    {
        public int Id { get; set; }

        public float Drive { get; set; }
        public float Backhand { get; set; }
        public float Smash { get; set; }
        public float Serve { get; set; }
        public float Vibora { get; set; }
        public float Bandeja { get; set; }

        // Navigation property for Player (one player per stats entry)
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        // Add methods here as needed
    }
}