using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.Models;

namespace padelya_api.Services
{
    public class PermissionService(PadelYaDbContext context) : IPermissionService
    {
        private readonly PadelYaDbContext _context = context;

        public async Task<bool> HasPermissionAsync(int userId, string permissionName)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                    .ThenInclude(p => (p as SimplePermission).Module)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Role == null) return false;

            return CheckPermissionRecursively(user.Role, permissionName);
        }

        public async Task<bool> HasModulePermissionAsync(int userId, string module, string action)
        {
            var permissionName = $"{module}:{action}";
            return await HasPermissionAsync(userId, permissionName);
        }

        public async Task<bool> HasModuleAccessAsync(int userId, string module)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                    .ThenInclude(p => (p as SimplePermission).Module)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Role == null) return false;

            var permissions = new HashSet<string>();
            CollectPermissionsRecursively(user.Role, permissions);

            return permissions.Any(p => p.StartsWith($"{module}:"));
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                    .ThenInclude(p => (p as SimplePermission).Module)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Role == null) return new List<string>();

            var permissions = new HashSet<string>();
            CollectPermissionsRecursively(user.Role, permissions);
            return permissions.ToList();
        }

        public async Task<List<string>> GetUserModulePermissionsAsync(int userId, string module)
        {
            var allPermissions = await GetUserPermissionsAsync(userId);
            return allPermissions.Where(p => p.StartsWith($"{module}:")).ToList();
        }

        public async Task<bool> HasAnyPermissionAsync(int userId, params string[] permissionNames)
        {
            // Cargar todos los permisos del usuario una sola vez
            var userPermissions = await GetUserPermissionsAsync(userId);

            // Verificar si tiene al menos uno de los permisos requeridos
            return permissionNames.Any(permission => userPermissions.Contains(permission));
        }

        public async Task<bool> HasAllPermissionsAsync(int userId, params string[] permissionNames)
        {
            // Cargar todos los permisos del usuario una sola vez
            var userPermissions = await GetUserPermissionsAsync(userId);

            // Verificar que tenga todos los permisos requeridos
            return permissionNames.All(permission => userPermissions.Contains(permission));
        }

        private bool CheckPermissionRecursively(PermissionComponent component, string permissionName)
        {
            if (component is SimplePermission simple)
            {
                return simple.Name == permissionName;
            }
            else if (component is RolComposite composite)
            {
                return composite.Permissions.Any(p => CheckPermissionRecursively(p, permissionName));
            }
            return false;
        }

        private void CollectPermissionsRecursively(PermissionComponent component, HashSet<string> permissions)
        {
            if (component is SimplePermission simple)
            {
                permissions.Add(simple.Name);
            }
            else if (component is RolComposite composite)
            {
                foreach (var perm in composite.Permissions)
                {
                    CollectPermissionsRecursively(perm, permissions);
                }
            }
        }
    }
}