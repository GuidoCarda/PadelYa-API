using System;

namespace padelya_api.DTOs.Tournament
{
    public class UpdateTournamentDto
    {
        public string? Title { get; set; }
        public string? Category { get; set; }
        public int? Quota { get; set; }
        public decimal? EnrollmentPrice { get; set; }
        public DateTime? EnrollmentStartDate { get; set; }
        public DateTime? EnrollmentEndDate { get; set; }
        public DateTime? TournamentStartDate { get; set; }
        public DateTime? TournamentEndDate { get; set; }
    }
}
