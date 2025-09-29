using System;
using System.Collections.Generic;
using padelya_api.Models;
using padelya_api.Models.Tournament;

namespace padelya_api.Models.Tournament
{
    public class TournamentEnrollment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        // Player who is being enrolled (used by service logic)
        public int PlayerId { get; set; }

        // Navigation property for the couple (should contain 2 players)
        public int CoupleId { get; set; }
        public Couple Couple { get; set; }

        // Navigation property for Tournament
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        // Enrollment timestamp used by service logic
        public DateTime EnrollmentDate { get; set; }

        public Payment Payment { get; set; }
        // Add methods here as needed
    }
}