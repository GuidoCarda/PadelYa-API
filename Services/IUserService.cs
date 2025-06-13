using padelya_api.Models;
using padelya_api.DTOs;

namespace padelya_api.Services
{
  public interface IUserService
  {
    // Basic CRUD
   Task<IEnumerable<User>> GetUsersAsync(string? search = null, int? statusId = null);
   Task<User?> GetUserByIdAsync(int id);
   Task<User?> CreateUserAsync(CreateUserDto request);
   Task<User?> UpdateUserAsync(int id, UpdateUserDto userDto);
   Task<RolComposite?> GetUserRoleAsync(int userId);

   Task<bool> DeleteUserAsync(int id);
   Task<bool> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto);
   // Status Management
   //Task<bool> ActivateUserAsync(int id);
   //Task<bool> DeactivateUserAsync(int id);

   // Role Management
   //Task<bool> AssignRoleToUserAsync(int userId, int roleId);
    }
}

