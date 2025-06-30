using padelya_api.Models;

namespace padelya_api.DTOs.Auth
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public Person? Person { get; set; }
        public List<string> Permissions { get; set; } = new();
        public List<string> Modules { get; set; } = new();
    }
}
