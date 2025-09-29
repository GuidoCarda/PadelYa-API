using System;
using System.Collections.Generic;
using padelya_api.Constants;

namespace padelya_api.Models.Tournament
{
    public class Tournament
    {
        public int Id { get; set; }

        // Scalar properties
        public string CurrentPhase { get; set; }
        public TournamentStatus TournamentStatus { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int Quota { get; set; }
        public decimal EnrollmentPrice { get; set; }
        public DateTime EnrollmentStartDate { get; set; }
        public DateTime EnrollmentEndDate { get; set; }
        public DateTime TournamentStartDate { get; set; }
        public DateTime TournamentEndDate { get; set; }

        // Navigation properties
        public List<TournamentEnrollment> Enrollments { get; set; }
        public List<TournamentPhase> TournamentPhases { get; set; }

    }
}
