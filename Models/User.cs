using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Primitives;

namespace padelya_api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public int StatusId { get; set; }
        public UserStatus Status { get; set; }

        public int RoleId { get; set; }
        public RolComposite Role { get; set; }

        public int? PersonId { get; set; } // relacion opcional
        public Person? Person { get; set; }
    }
}
