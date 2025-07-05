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

        // Navigation property for Booking (assuming one booking per match)
        // public int BookingId { get; set; }
        // public Booking.Booking Booking { get; set; }
        // Add methods here as needed
    }
}