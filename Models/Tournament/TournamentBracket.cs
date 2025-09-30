using System.Collections.Generic;

namespace padelya_api.Models.Tournament
{
    public class TournamentBracket
    {
        public int Id { get; set; }
        public int PhaseId { get; set; }

        // Navigation properties
        public TournamentPhase Phase { get; set; } = null!;
        public List<TournamentMatch> Matches { get; set; } = new();
    }
}