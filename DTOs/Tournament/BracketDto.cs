namespace padelya_api.DTOs.Tournament
{
    public class TournamentMatchDto
    {
        public int Id { get; set; }
        public string TournamentMatchState { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public int? CoupleOneId { get; set; }
        public int? CoupleTwoId { get; set; }
        public CoupleResponseDto? CoupleOne { get; set; }
        public CoupleResponseDto? CoupleTwo { get; set; }
        public int? CourtSlotId { get; set; }
        public int MatchNumber { get; set; }
        public int? WinnerCoupleId { get; set; }
    }

    public class TournamentBracketDto
    {
        public int Id { get; set; }
        public int PhaseId { get; set; }
        public List<TournamentMatchDto> Matches { get; set; } = new();
    }

    public class TournamentPhaseWithBracketsDto
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TournamentBracketDto> Brackets { get; set; } = new();
    }

    public class GenerateBracketResponseDto
    {
        public int TournamentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalPhases { get; set; }
        public int TotalMatches { get; set; }
        public List<TournamentPhaseWithBracketsDto> Phases { get; set; } = new();
    }
}

