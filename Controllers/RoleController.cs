using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Role;
using padelya_api.Models;
using padelya_api.Services;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
  [Route("api/roles")]
  [ApiController]
  // [Authorize(Roles = "Admin")]
  public class RoleController(IRoleService roleService) : ControllerBase
  {
    // GET: api/roles
    [HttpGet]
    public async Task<ActionResult<ResponseMessage<IEnumerable<RoleWithUserCountDto>>>> GetRoles()
    {
      try
      {
        var roles = await roleService.GetRolesAsync();
        var response = ResponseMessage<IEnumerable<RoleWithUserCountDto>>.SuccessResult(roles, "Roles retrieved successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<IEnumerable<RolComposite>>.Error($"Error retrieving roles: {ex.Message}", "ROLES_RETRIEVAL_ERROR");
        return StatusCode(500, response);
      }
    }

    // GET: api/roles/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseMessage<RolComposite>>> GetRole(int id)
    {
      try
      {
        var role = await roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
          var notFoundResponse = ResponseMessage<RolComposite>.NotFound($"Role with ID {id} not found");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage<RolComposite>.SuccessResult(role, "Role retrieved successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<RolComposite>.Error($"Error retrieving role: {ex.Message}", "ROLE_RETRIEVAL_ERROR");
        return StatusCode(500, response);
      }
    }

    // POST: api/roles
    [HttpPost]
    public async Task<ActionResult<ResponseMessage<RolComposite>>> CreateRole([FromBody] CreateRoleDto roleDto)
    {
      try
      {
        // Simple validation
        var validationErrors = ValidateCreateRole(roleDto);
        if (validationErrors.Any())
        {
          var validationResponse = ResponseMessage<RolComposite>.ValidationError("Invalid input data", validationErrors);
          return BadRequest(validationResponse);
        }

        var role = await roleService.CreateRoleAsync(roleDto);
        if (role == null)
        {
          var conflictResponse = ResponseMessage<RolComposite>.Conflict("No se pudo crear el rol");
          return Conflict(conflictResponse);
        }

        var response = ResponseMessage<RolComposite>.SuccessResult(role, "Role created successfully");
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<RolComposite>.Error($"Error creating role: {ex.Message}", "ROLE_CREATION_ERROR");
        return StatusCode(500, response);
      }
    }

    // PUT: api/roles/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseMessage<RolComposite>>> UpdateRole(int id, [FromBody] UpdateRoleDto roleDto)
    {
      try
      {
        // Check if at least one field is provided
        if (string.IsNullOrWhiteSpace(roleDto.Name))
        {
          var validationResponse = ResponseMessage<RolComposite>.ValidationError(
              "At least one field must be provided for update",
              new List<ValidationError>
              {
                            new ValidationError("Request", "Please provide a name to update")
              }
          );
          return BadRequest(validationResponse);
        }

        // Simple validation for provided fields
        var validationErrors = ValidateUpdateRole(roleDto);
        if (validationErrors.Any())
        {
          var validationResponse = ResponseMessage<RolComposite>.ValidationError("Invalid input data", validationErrors);
          return BadRequest(validationResponse);
        }

        var role = await roleService.UpdateRoleAsync(id, roleDto);
        if (role == null)
        {
          var notFoundResponse = ResponseMessage<RolComposite>.NotFound($"Role with ID {id} not found");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage<RolComposite>.SuccessResult(role, "Role updated successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<RolComposite>.Error($"Error updating role: {ex.Message}", "ROLE_UPDATE_ERROR");
        return StatusCode(500, response);
      }
    }

    // DELETE: api/roles/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseMessage>> DeleteRole(int id)
    {
      try
      {
        var result = await roleService.DeleteRoleAsync(id);
        if (!result)
        {
          var notFoundResponse = ResponseMessage.NotFound($"Role with ID {id} not found or could not be deleted");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage.SuccessMessage("Role deleted successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage.Error($"Error deleting role: {ex.Message}", "ROLE_DELETION_ERROR");
        return StatusCode(500, response);
      }
    }

    // GET: api/roles/{id}/permissions
    [HttpGet("{id}/permissions")]
    public async Task<ActionResult<ResponseMessage<IEnumerable<object>>>> GetRolePermissions(int id)
    {
      try
      {
        var permissions = await roleService.GetRolePermissionsAsync(id);
        var response = ResponseMessage<IEnumerable<object>>.SuccessResult(permissions, "Role permissions retrieved successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<IEnumerable<object>>.Error($"Error retrieving role permissions: {ex.Message}", "ROLE_PERMISSIONS_RETRIEVAL_ERROR");
        return StatusCode(500, response);
      }
    }

    // POST: api/roles/{id}/permissions
    [HttpPost("{id}/permissions")]
    public async Task<ActionResult<ResponseMessage>> AddPermissionToRole(int id, [FromBody] AddPermissionDto permissionDto)
    {
      try
      {
        // Simple validation
        var validationErrors = ValidateAddPermission(permissionDto);
        if (validationErrors.Any())
        {
          var validationResponse = ResponseMessage.ValidationError("Invalid input data", validationErrors);
          return BadRequest(validationResponse);
        }

        var result = await roleService.AddPermissionsToRoleAsync(id, permissionDto.permissionsIds);

        if (result == AddPermissionResult.AlreadyExists)
        {
          var conflictResponse = ResponseMessage.Conflict("The role already has this permission");
          return Conflict(conflictResponse);
        }

        if (result == AddPermissionResult.RoleNotFound)
        {
          var notFoundResponse = ResponseMessage.NotFound("Role not found");
          return NotFound(notFoundResponse);
        }

        if (result == AddPermissionResult.PermissionNotFound)
        {
          var notFoundResponse = ResponseMessage.NotFound("Permission not found");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage.SuccessMessage("Permissions added successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage.Error($"Error adding permissions to role: {ex.Message}", "ADD_PERMISSIONS_ERROR");
        return StatusCode(500, response);
      }
    }

    // DELETE: api/roles/{id}/permissions/{permissionId}
    [HttpDelete("{id}/permissions/{permissionId}")]
    public async Task<ActionResult<ResponseMessage>> RemovePermissionFromRole(int id, int permissionId)
    {
      try
      {
        var result = await roleService.RemovePermissionFromRoleAsync(id, permissionId);
        if (!result)
        {
          var notFoundResponse = ResponseMessage.NotFound("Role or permission not found");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage.SuccessMessage("Permission removed successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage.Error($"Error removing permission from role: {ex.Message}", "REMOVE_PERMISSION_ERROR");
        return StatusCode(500, response);
      }
    }

    // GET: api/roles/{id}/users
    [HttpGet("{id}/users")]
    public async Task<ActionResult<ResponseMessage<IEnumerable<object>>>> GetUsersByRole(int id)
    {
      try
      {
        var users = await roleService.GetUsersByRoleAsync(id);
        var response = ResponseMessage<IEnumerable<object>>.SuccessResult(users, "Users by role retrieved successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<IEnumerable<object>>.Error($"Error retrieving users by role: {ex.Message}", "USERS_BY_ROLE_RETRIEVAL_ERROR");
        return StatusCode(500, response);
      }
    }


    [HttpGet("modules")]
    [HttpGet]
    public async Task<ActionResult<ResponseMessage<List<ModulePermissionsDto>>>> GetModules()
    {
      try
      {
        var roles = await roleService.GetPermissions();
        var response = ResponseMessage<List<ModulePermissionsDto>>.SuccessResult(roles, "Modules retrieved successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<List<ModulePermissionsDto>>.Error($"Error retrieving modules: {ex.Message}", "ROLES_RETRIEVAL_ERROR");
        return StatusCode(500, response);
      }
    }

    #region Simple Validation Methods

    /// <summary>
    /// Simple validation for creating a role
    /// </summary>
    private List<ValidationError> ValidateCreateRole(CreateRoleDto roleDto)
    {
      var errors = new List<ValidationError>();

      if (string.IsNullOrWhiteSpace(roleDto.Name))
        errors.Add(new ValidationError("Name", "Role name is required"));

      if (!string.IsNullOrWhiteSpace(roleDto.Name) && roleDto.Name.Length > 100)
        errors.Add(new ValidationError("Name", "Role name cannot exceed 100 characters"));

      return errors;
    }

    /// <summary>
    /// Simple validation for updating a role
    /// </summary>
    private List<ValidationError> ValidateUpdateRole(UpdateRoleDto roleDto)
    {
      var errors = new List<ValidationError>();

      if (!string.IsNullOrWhiteSpace(roleDto.Name) && roleDto.Name.Length > 100)
        errors.Add(new ValidationError("Name", "Role name cannot exceed 100 characters"));

      return errors;
    }

    /// <summary>
    /// Simple validation for adding permissions
    /// </summary>
    private List<ValidationError> ValidateAddPermission(AddPermissionDto permissionDto)
    {
      var errors = new List<ValidationError>();

      if (permissionDto.permissionsIds == null || permissionDto.permissionsIds.Count == 0)
        errors.Add(new ValidationError("permissionsIds", "At least one permission ID is required"));

      return errors;
    }

    #endregion
  }
}