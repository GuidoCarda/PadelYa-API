namespace padelya_api.DTOs.Auth
{
  public class RegisterUserDto
  {
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime Birthdate { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
  }

  public class RegisterPlayerDto : RegisterUserDto
  {
    public string Category { get; set; }
    public string PreferredPosition { get; set; }
  }

  public class RegisterTeacherDto : RegisterUserDto
  {
    public string Title { get; set; }
    public string Institution { get; set; }
    public string Category { get; set; }
  }
}
