using System.Collections.Generic;
using padelya_api.models;
using padelya_api.Models;

namespace padelya_api.Models.Class
{
    public class Routine
    {
        public int Id { get; set; }
        public TimeOnly Duration { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        // Navigation property for the creator (Teacher)
        public int CreatorId { get; set; }
        public Teacher Creator { get; set; }

        // Navigation property for Players (1 or more)
        public List<Player> Players { get; set; }

        // Navigation property for Excercises (1 or more)
        public List<Exercise> Exercises { get; set; }

        // Add methods here as needed
    }
}