namespace padelya_api.DTOs
{
  public class UserDto
  {
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }

  public class PlayerRegisterDto : UserDto
  {
    public DateTime BirthDate { get; set; }
    public string PreferredPosition { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
  }

  public class TeacherRegisterDto : UserDto
  {
    public string Title { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
  }

  public class CreateUserDto
  {
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public int RoleId { get; set; }
  }

  public class UpdateUserDto
  {
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
  }


   public class ChangePasswordDto
   {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
   }

  public class AssignRoleDto
  {
    public int RoleId { get; set; }
  }
}