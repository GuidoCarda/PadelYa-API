using System.Text.Json.Serialization;

namespace padelya_api.DTOs.User
{
    /// <summary>
    /// DTO for creating a new user
    /// </summary>
    public class CreateUserDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public PersonDto? Person { get; set; }
    }

    /// <summary>
    /// DTO for updating user information (partial update)
    /// </summary>
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public string? PhoneNumber { get; set; } // For updating Person.PhoneNumber
    }

    /// <summary>
    /// DTO for booking statistics related to a user
    /// </summary>
    public class UserBookingStatsDto
    {
        public int ActiveCount { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// DTO for user information returned by API
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public PersonDto? Person { get; set; }
        public List<string> Permissions { get; set; }
        public UserBookingStatsDto? Bookings { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime RegisteredAt { get; set; }
    }



    /// <summary>
    /// DTO for changing user password
    /// </summary>
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating user status
    /// </summary>
    public class UpdateUserStatusDto
    {
        public int StatusId { get; set; }
    }

    /// <summary>
    /// DTO for updating user profile (self-service)
    /// </summary>
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        
        // Person fields (for Player/Teacher users)
        public string? PhoneNumber { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Category { get; set; }
        
        // Player-specific field
        public string? PreferredPosition { get; set; }
        
        // Teacher-specific fields
        public string? Title { get; set; }
        public string? Institution { get; set; }
    }

    [JsonDerivedType(typeof(PlayerDto), "player")]
    [JsonDerivedType(typeof(TeacherDto), "teacher")]
    public class PersonDto
    {
        public int Id { get; set; }
        public string PersonType { get; set; }
        public DateTime Birthdate { get; set; }
        public string Category { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class PlayerDto : PersonDto
    {
        public string PreferredPosition { get; set; }
    }

    public class TeacherDto : PersonDto
    {
        public string Title { get; set; }
        public string Institution { get; set; }
    }


    public class LoggedInUserDTO
    {
        public UserDto User { get; set; }
        public PersonDto Person { get; set; }
    }

    public class UserLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}