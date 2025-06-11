using padelya_api.Models;
using padelya_api.DTOs;

namespace padelya_api.Services
{
  public interface IUserService
  {
    // Basic CRUD
    IEnumerable<User> GetUsers(string? search = null, int? statusId = null);
    //Task<User?> GetUserByIdAsync(int id);
    //Task<User> CreateUserAsync(CreateUserDto userDto);
    //Task<User?> UpdateUserAsync(int id, UpdateUserDto userDto);
    //Task<bool> DeleteUserAsync(int id);

    // Status Management
    //Task<bool> ActivateUserAsync(int id);
    //Task<bool> DeactivateUserAsync(int id);

    // Role Management
    //Task<bool> AssignRoleToUserAsync(int userId, int roleId);
  }
}
