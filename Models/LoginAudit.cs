namespace padelya_api.Models
{
    public class LoginAudit
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public LoginAuditAction Action { get; set; }
        public DateTime Timestamp { get; set; }

        // Información adicional útil
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Notes { get; set; }
    }

    public enum LoginAuditAction
    {
        Login,
        Logout,
        RefreshToken,
        FailedLogin
    }
}

