namespace padelya_api.DTOs
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }

        public List<string> Permissions { get; set; } = new();
        public List<string> Forms { get; set; } = new();
    }
}
