using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Role;
using padelya_api.Models;

namespace padelya_api.Services
{
    public enum AddPermissionResult
    {
        Success,
        RoleNotFound,
        PermissionNotFound,
        AlreadyExists
    }

    public class RoleService : IRoleService
    {
        private readonly PadelYaDbContext _context;

        public RoleService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<List<RolComposite>> GetRolesAsync()
        {
            var roles = await _context.PermissionComponents
                      .OfType<RolComposite>()
                      .Include(r => r.Permissions)
                      .ToListAsync();
            return roles;
        }



        public async Task<RolComposite?> GetRoleByIdAsync(int roleId)
        {
            var role = await _context.PermissionComponents
                .OfType<RolComposite>()
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);
            return role;
        }

        public async Task<RolComposite?> CreateRoleAsync(CreateRoleDto roleDto)
        {

            if (string.IsNullOrWhiteSpace(roleDto.Name))
            {
                return null;
            }

            var role = new RolComposite
            {
                Name = roleDto.Name
            };

            _context.PermissionComponents.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<RolComposite?> UpdateRoleAsync(int id, UpdateRoleDto roleDto)
        {

            if (string.IsNullOrWhiteSpace(roleDto.Name))
            {
                return null;
            }

            var role = await _context.PermissionComponents
                .OfType<RolComposite>()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return null;

            role.Name = roleDto.Name;
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.PermissionComponents
                .OfType<RolComposite>()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return false;

            _context.PermissionComponents.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PermissionComponent>> GetRolePermissionsAsync(int roleId)
        {
            var role = await _context.PermissionComponents
                .OfType<RolComposite>()
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            return role?.Permissions ?? new List<PermissionComponent>();
        }

        public async Task<AddPermissionResult> AddPermissionsToRoleAsync(int roleId, List<int> permissionsIds)
        {
            var role = await _context.PermissionComponents
                .OfType<RolComposite>()
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                return AddPermissionResult.RoleNotFound;
            }

            var permissions = await _context.PermissionComponents
                  .Where(p => permissionsIds.Contains(p.Id))
                  .ToListAsync();

            if (permissions.Count == 0)
            {
                return AddPermissionResult.PermissionNotFound;
            }

            bool anyAdded = false;
            foreach (var permission in permissions)
            {
                if (!RoleHasPermission(role, permission.Id))
                {
                    role.Permissions.Add(permission);
                    anyAdded = true;
                }
            }

            if (anyAdded)
            {
                await _context.SaveChangesAsync();
            }

            return AddPermissionResult.Success;
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var role = await _context.PermissionComponents
                .OfType<RolComposite>()
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            var permission = await _context.PermissionComponents
                .FirstOrDefaultAsync(p => p.Id == permissionId);

            if (role == null || permission == null)
                return false;

            role.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _context.Users
                .Where(u => u.RoleId == roleId)
                .ToListAsync();
        }

        public bool RoleHasPermission(PermissionComponent component, int permissionId)
        {
            if (component.Id == permissionId)
                return true;

            if (component is RolComposite composite)
            {
                foreach (var child in composite.Permissions)
                {
                    if (RoleHasPermission(child, permissionId))
                        return true;
                }
            }

            return false;
        }
    }
}
