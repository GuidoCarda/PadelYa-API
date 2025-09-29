using System.Collections.Generic;
using padelya_api.Models;

namespace padelya_api.Models.Tournament
{
    public class TournamentMatch
    {
        public int Id { get; set; }

        public string TournamentMatchState { get; set; }
        public string Result { get; set; }

        // Navigation properties for couples
        public int CoupleOneId { get; set; } // Should contain 2 players
        public Couple CoupleOne { get; set; } // Should contain 2 players
        public int CoupleTwoId { get; set; } // Should contain 2 players
        public Couple CoupleTwo { get; set; } // Should contain 2 players


        public int BracketId { get; set; }

        // Navigation property for CourtSlot
        public int CourtSlotId { get; set; }
        public CourtSlot CourtSlot { get; set; }
    }
}