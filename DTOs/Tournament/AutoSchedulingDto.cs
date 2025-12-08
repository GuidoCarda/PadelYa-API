using System;
using System.Collections.Generic;

namespace padelya_api.DTOs.Tournament
{
    public class AutoSchedulingResultDto
    {
        public int TournamentId { get; set; }
        public int TotalMatchesScheduled { get; set; }
        public int TotalMatchesFailed { get; set; }
        public List<ScheduledMatchInfo> ScheduledMatches { get; set; } = new();
        public List<FailedMatchInfo> FailedMatches { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool HasConflicts => TotalMatchesFailed > 0;

    }

    public class ScheduledMatchInfo
    {
        public int MatchId { get; set; }
        public int CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class FailedMatchInfo
    {
        public int MatchId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class AvailableSlotDto
    {
        public int CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

