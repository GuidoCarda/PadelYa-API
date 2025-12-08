namespace padelya_api.Models
{
  public class User
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public int StatusId { get; set; }
    public UserStatus Status { get; set; }

    public int RoleId { get; set; }
    public RolComposite Role { get; set; }

    public int? PersonId { get; set; } // relacion opcional
    public Person? Person { get; set; }

    public DateTime RegisteredAt { get; set; }
  }
}
