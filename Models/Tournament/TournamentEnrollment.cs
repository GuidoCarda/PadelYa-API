using System;
using padelya_api.Models;

namespace padelya_api.Models.Tournament
{
    public class TournamentEnrollment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int PlayerId { get; set; }

        // Foreign Keys
        public int CoupleId { get; set; }
        public int TournamentId { get; set; }

        // Navigation properties
        public Couple Couple { get; set; } = null!;
        public Tournament Tournament { get; set; } = null!;
        public Payment? Payment { get; set; } 
    }
}
