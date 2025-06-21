using padelya_api.DTOs.User;

namespace padelya_api.DTOs.Auth
{
    public class LoggedInUserDto
    {
        public required UserDto User { get; set; }
        public required PersonDto Person { get; set; }
    }
}
