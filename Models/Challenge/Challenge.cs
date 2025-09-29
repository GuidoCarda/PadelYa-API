using System;

namespace padelya_api.Models.Challenge
{
    public class Challenge
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int RequesterPlayerId { get; set; }
        public int RequesterPartnerPlayerId { get; set; }
        public int TargetPlayerId { get; set; }
        public int TargetPartnerPlayerId { get; set; }
        public ChallengeStatus Status { get; set; } = ChallengeStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
        public DateTime? PlayedAt { get; set; }

        public int? WinnerPlayerId { get; set; }
        public int? WinnerPartnerPlayerId { get; set; }

        public string? Sets { get; set; } // Ej. "6-4,3-6,7-5"
        public int RequesterPointsAtCreation { get; set; }
        public int TargetPointsAtCreation { get; set; }
        public int PointsAwardedPerPlayer { get; set; }
        public int? ValidatedByAdminUserId { get; set; }
        public DateTime? ValidatedAt { get; set; }
    }
}

