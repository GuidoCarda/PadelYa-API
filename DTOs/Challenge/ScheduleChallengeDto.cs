using System;

namespace padelya_api.DTOs.Challenge
{
    public class ScheduleChallengeDto
    {
        public int CourtId { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}
