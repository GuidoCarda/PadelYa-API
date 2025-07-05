
using System.Collections.Generic;

namespace padelya_api.Models.Tournament
{
    public class TournamentPhase
    {
        public int Id { get; set; }

        // Navigation property
        public List<TournamentBracket> Brackets { get; set; }

        public int TournamentId { get; set; }
        // Add methods here as needed
    }
}