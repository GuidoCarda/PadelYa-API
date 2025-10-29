using System.Collections.Generic;
using padelya_api.Models;
using BookingModels = padelya_api.Models.Booking;

namespace padelya_api.Models.Tournament
{
    public class TournamentMatch
    {
        public int Id { get; set; }
        
        // Scalar properties
        public string TournamentMatchState { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;

        // Foreign Keys (nullable para permitir partidos sin parejas o cancha asignadas)
        public int? CoupleOneId { get; set; }
        public int? CoupleTwoId { get; set; }
        public int BracketId { get; set; }
        public int? CourtSlotId { get; set; }

        // Navigation properties (EF Core las inicializará)
        public Couple? CoupleOne { get; set; }
        public Couple? CoupleTwo { get; set; }
        public TournamentBracket Bracket { get; set; } = null!;
        public CourtSlot? CourtSlot { get; set; }
    }
}