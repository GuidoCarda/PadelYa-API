
using System;
using System.Collections.Generic;

namespace padelya_api.Models.Tournament
{
    public class TournamentPhase
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        
        // Propiedades de la fase
        public string PhaseName { get; set; } = string.Empty;
        public int PhaseOrder { get; set; } // Orden de la fase (1 = primera ronda, 2 = segunda, etc.)
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation properties
        public Tournament Tournament { get; set; } = null!;
        public List<TournamentBracket> Brackets { get; set; } = new();
    }
}