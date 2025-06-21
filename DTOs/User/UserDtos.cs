using System.Text.Json.Serialization;

namespace padelya_api.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public PersonDto? Person { get; set; }
        public List<string> Permissions { get; set; }
    }

    [JsonDerivedType(typeof(PlayerDto), "player")]
    [JsonDerivedType(typeof(TeacherDto), "teacher")]
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PersonType { get; set; }
    }

    public class PlayerDto : PersonDto
    {
        public string Category { get; set; }
        public string PreferredPosition { get; set; }
    }

    public class TeacherDto : PersonDto
    {
        public string Title { get; set; }
        public string Institution { get; set; }
        public string Category { get; set; }
    }


    public class LoggedInUserDTO
    {
        public UserDto User { get; set; }
        public PersonDto Person { get; set; }
    }



    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

    public class UpdateUserDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? Surname { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int? RoleId { get; set; }
    }

    public class UserLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }


    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }


}