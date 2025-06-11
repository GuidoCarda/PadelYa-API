using padelya_api.DTOs;
using padelya_api.Models;

namespace padelya_api.Services
{
  public interface IRoleService
  {
    Task<List<RolComposite>> GetRolesAsync();
    Task<RolComposite?> GetUserRoleAsync(int userId);
    Task<RolComposite?> GetRoleByIdAsync(int roleId);
    Task<RolComposite> CreateRoleAsync(CreateRoleDto roleDto);
    Task<RolComposite?> UpdateRoleAsync(int id, UpdateRoleDto roleDto);
    Task<bool> DeleteRoleAsync(int id);
    Task<IEnumerable<PermissionComponent>> GetRolePermissionsAsync(int roleId);
    Task<bool> AddPermissionToRoleAsync(int roleId, int permissionId);
    Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
  }
}
