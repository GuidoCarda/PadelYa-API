using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.Models;
using padelya_api.DTOs;

namespace padelya_api.Services
{
    public enum AddPermissionResult
    {
        Success,
        RoleNotFound,
        PermissionNotFound,
        AlreadyExists
    }

    public class RoleService(PadelYaDbContext context, IConfiguration configuration) : IRoleService
    {
    private readonly PadelYaDbContext _context = context;
    private readonly IConfiguration _configuration;

    public async Task<List<RolComposite>> GetRolesAsync()
    {
      var roles = await _context.PermissionComponents
                .OfType<RolComposite>()
                .Include(r=> r.Permissions)
                .ToListAsync();
      return roles;
    }

    public async Task<RolComposite?> GetUserRoleAsync(int userId)
    {
        var user = await _context.Users
          .Include(u => u.Role)
              .ThenInclude(r => r.Permissions)
          .FirstOrDefaultAsync(u => u.Id == userId);

      return user?.Role;
    }

    public async Task<RolComposite?> GetRoleByIdAsync(int roleId)
    {
      var role = await _context.PermissionComponents
          .OfType<RolComposite>()
          .Include(r => r.Permissions)
          .FirstOrDefaultAsync(r => r.Id == roleId);
      return role;
    }

    public async Task<RolComposite> CreateRoleAsync(CreateRoleDto roleDto)
    {
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

    public async Task<AddPermissionResult> AddPermissionToRoleAsync(int roleId, int permissionId)
    {
      var role = await _context.PermissionComponents
          .OfType<RolComposite>()
          .Include(r => r.Permissions)
          .FirstOrDefaultAsync(r => r.Id == roleId);

    if (role == null)
    {
        return AddPermissionResult.RoleNotFound;
    }


    var permission = await _context.PermissionComponents
          .FirstOrDefaultAsync(p => p.Id == permissionId);


     
    if (permission == null)
    {
        return AddPermissionResult.PermissionNotFound;
    }

    if(RoleHasPermission(role,permissionId))
    {
        return AddPermissionResult.AlreadyExists;
    }

      role.Permissions.Add(permission);
      await _context.SaveChangesAsync();

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
