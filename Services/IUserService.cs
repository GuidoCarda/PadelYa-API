using padelya_api.DTOs.User;
using padelya_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace padelya_api.Services
{
    public interface IUserService
    {
        // Basic CRUD
        Task<IEnumerable<UserDto>> GetUsersAsync(string? search = null, int? statusId = null);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> CreateUserAsync(CreateUserDto request);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto userDto);
        Task<RolComposite?> GetUserRoleAsync(int userId);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto);
        Task<IEnumerable<UserDto>> SearchPlayersByEmailAsync(string email);
        // Status Management
        Task<bool> UpdateUserStatusAsync(int id, int statusId);

        // Profile Management (self-service)
        Task<UserDto?> UpdateProfileAsync(int id, UpdateProfileDto profileDto);

        // Role Management
        //Task<bool> AssignRoleToUserAsync(int userId, int roleId);
    }
}

