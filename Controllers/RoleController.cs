using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.Services;
using padelya_api.DTOs;
using padelya_api.Models;

namespace padelya_api.Controllers
{
  [Route("api/roles")]
  [ApiController]
  // [Authorize(Roles = "Admin")]
  public class RoleController(IRoleService roleService) : ControllerBase
  {
    // GET: api/roles
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
      var roles = await roleService.GetRolesAsync();
      return Ok(roles);
    }

    // GET: api/roles/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(int id)
    {
      var role = await roleService.GetRoleByIdAsync(id);
      if (role == null)
        return NotFound();
      return Ok(role);
    }

    // POST: api/roles
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto roleDto)
    {
      var role = await roleService.CreateRoleAsync(roleDto);
      return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
    }

    // PUT: api/roles/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto roleDto)
    {
      var role = await roleService.UpdateRoleAsync(id, roleDto);
      if (role == null)
        return NotFound();
      return Ok(role);
    }

    // DELETE: api/roles/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
      var result = await roleService.DeleteRoleAsync(id);
      if (!result)
        return NotFound();
      return NoContent();
    }

    // GET: api/roles/{id}/permissions
    [HttpGet("{id}/permissions")]
    public async Task<IActionResult> GetRolePermissions(int id)
    {
      var permissions = await roleService.GetRolePermissionsAsync(id);
      return Ok(permissions);
    }

    // POST: api/roles/{id}/permissions
    [HttpPost("{id}/permissions")]
    public async Task<IActionResult> AddPermissionToRole(int id, [FromBody] AddPermissionDto permissionDto)
    {
            var result = await roleService.AddPermissionToRoleAsync(id, permissionDto.PermissionId);
            
            if (result == AddPermissionResult.AlreadyExists)
                return Conflict(new { message = "El rol ya tiene este permiso." });

            if (result == AddPermissionResult.RoleNotFound)
                return NotFound(new { message = "Rol no encontrado." });

            if (result == AddPermissionResult.PermissionNotFound)
                return NotFound(new { message = "Permiso no encontrado." });
        
        return  CreatedAtAction(nameof(AddPermissionToRole), result); ;
     }


    // DELETE: api/roles/{id}/permissions/{permissionId}
    [HttpDelete("{id}/permissions/{permissionId}")]
    public async Task<IActionResult> RemovePermissionFromRole(int id, int permissionId)
    {
      var result = await roleService.RemovePermissionFromRoleAsync(id, permissionId);
      if (!result)
        return NotFound();
      return NoContent();
    }

    // GET: api/roles/{id}/users
    [HttpGet("{id}/users")]
    public async Task<IActionResult> GetUsersByRole(int id)
    {
      var users = await roleService.GetUsersByRoleAsync(id);
      return Ok(users);
    }

   
    }

    }