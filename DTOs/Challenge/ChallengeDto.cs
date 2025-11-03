using padelya_api.Models.Challenge;

namespace padelya_api.DTOs.Challenge
{
    public class ChallengeDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int RequesterPlayerId { get; set; }
        public string? RequesterPlayerName { get; set; }
        public string? RequesterPlayerSurname { get; set; }
        public int RequesterPartnerPlayerId { get; set; }
        public string? RequesterPartnerName { get; set; }
        public string? RequesterPartnerSurname { get; set; }
        public int TargetPlayerId { get; set; }
        public string? TargetPlayerName { get; set; }
        public string? TargetPlayerSurname { get; set; }
        public int TargetPartnerPlayerId { get; set; }
        public string? TargetPartnerName { get; set; }
        public string? TargetPartnerSurname { get; set; }
        public ChallengeStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public DateTime? PlayedAt { get; set; }
        public int? WinnerPlayerId { get; set; }
        public string? WinnerPlayerName { get; set; }
        public int? WinnerPartnerPlayerId { get; set; }
        public string? WinnerPartnerName { get; set; }
        public string? Sets { get; set; }
        public int RequesterPointsAtCreation { get; set; }
        public int TargetPointsAtCreation { get; set; }
        public int PointsAwardedPerPlayer { get; set; }
        public int? ValidatedByAdminUserId { get; set; }
        public DateTime? ValidatedAt { get; set; }
        public bool RequiresValidation { get; set; }
    }
}

