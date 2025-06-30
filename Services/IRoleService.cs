using padelya_api.DTOs.Role;
using padelya_api.Models;

namespace padelya_api.Services
{
    public interface IRoleService
    {
        // Role management (CRUD)
        Task<List<RolComposite>> GetRolesAsync();
        Task<RolComposite?> GetRoleByIdAsync(int roleId);
        Task<RolComposite?> CreateRoleAsync(CreateRoleDto roleDto);
        Task<RolComposite?> UpdateRoleAsync(int id, UpdateRoleDto roleDto);
        Task<bool> DeleteRoleAsync(int id);

        // Role-permission management
        Task<IEnumerable<PermissionComponent>> GetRolePermissionsAsync(int roleId);
        Task<AddPermissionResult> AddPermissionsToRoleAsync(int roleId, List<int> permissionsIds);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);

        // User-role management
        Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);

        // Role permission checking (for role management, not user authorization)
        bool RoleHasPermission(PermissionComponent component, int permissionId);
    }
}
