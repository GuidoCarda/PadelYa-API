namespace padelya_api.DTOs.Challenge
{
    public class CreateChallengeDto
    {
        public int RequesterPlayerId { get; set; }
        public int RequesterPartnerPlayerId { get; set; }
        public int TargetPlayerId { get; set; }
        public int TargetPartnerPlayerId { get; set; }
    }

    public class RespondChallengeDto
    {
        public bool Accept { get; set; }
    }

    public class RegisterChallengeResultDto
    {
        public string Sets { get; set; } = string.Empty;
        public int WinnerPlayerId { get; set; }
        public int WinnerPartnerPlayerId { get; set; }
    }
}

