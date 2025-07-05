using System.Collections.Generic;

namespace padelya_api.Models.Tournament
{
    public class TournamentBracket
    {
        public int Id { get; set; }

        // Navigation property
        public List<TournamentMatch> Matches { get; set; }

        public int PhaseId { get; set; }
        // Add methods here as needed
    }
}