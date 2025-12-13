using System;
using System.Collections.Generic;
using padelya_api.Constants;
using System.ComponentModel.DataAnnotations.Schema;

namespace padelya_api.Models.Tournament
{
    public class Tournament
    {
        public int Id { get; set; }

        public string CurrentPhase { get; set; } = string.Empty;
        public TournamentStatus TournamentStatus { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quota { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal EnrollmentPrice { get; set; }
        public DateTime EnrollmentStartDate { get; set; }
        public DateTime EnrollmentEndDate { get; set; }
        public DateTime TournamentStartDate { get; set; }
        public DateTime TournamentEndDate { get; set; }

        public List<TournamentEnrollment> Enrollments { get; set; } = new();
        public List<TournamentPhase> TournamentPhases { get; set; } = new();
    }
}
