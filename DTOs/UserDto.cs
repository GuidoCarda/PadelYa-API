namespace padelya_api.DTOs
{
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class PlayerRegisterDto : UserDto {
        public DateTime BirthDate { get; set; }
        public string PreferredPosition { get; set; }
        public string Category { get; set; }
    }

    public class TeacherRegisterDto : UserDto { 
        public string Title { get; set; }
        public string Institution { get; set; }
        public string Category { get; set; }
    }

    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }

}
