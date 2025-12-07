using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.Attributes;
using padelya_api.Constants;
using padelya_api.DTOs.User;
using padelya_api.Models;
using padelya_api.Services;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
  /// <summary>
  /// Controller for managing user operations
  /// </summary>
  [Route("api/users")]
  [ApiController]
  //[Authorize] 
  // [RequireModuleAccess("user")]
  public class UserController(IUserService userService, IRoleService roleService) : ControllerBase
  {
    /// <summary>
    /// Get all users with optional search and status filtering
    /// </summary>
    /// <param name="search">Search term for filtering users</param>
    /// <param name="statusId">Status ID for filtering users</param>
    /// <returns>List of users matching the criteria</returns>
    [HttpGet]
    // [RequirePermission(Permissions.User.View)]
    // [RequireAnyPermission(Permissions.User.View, Permissions.User.EditSelf, Permissions.User.AssignRoles)]
    public async Task<ActionResult<ResponseMessage<IEnumerable<UserDto>>>> GetUsers(string? search = null, int? statusId = null)
    {
      try
      {
        var users = await userService.GetUsersAsync(search, statusId);
        var response = ResponseMessage<IEnumerable<UserDto>>.SuccessResult(users, "Users retrieved successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<IEnumerable<UserDto>>.Error($"Error retrieving users: {ex.Message}", "USERS_RETRIEVAL_ERROR");
        return StatusCode(500, response);
      }
    }

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    [HttpGet("{id}")]
    // [RequirePermission(Permissions.User.View)]
    public async Task<ActionResult<ResponseMessage<UserDto>>> GetUser(int id)
    {
      try
      {
        var user = await userService.GetUserByIdAsync(id);
        if (user == null)
        {
          var notFoundResponse = ResponseMessage<UserDto>.NotFound($"User with ID {id} not found");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage<UserDto>.SuccessResult(user, "User retrieved successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<UserDto>.Error($"Error retrieving user: {ex.Message}", "USER_RETRIEVAL_ERROR");
        return StatusCode(500, response);
      }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="userDto">User creation data</param>
    /// <returns>Created user information</returns>
    [HttpPost]
    // [RequirePermission(Permissions.User.Create)]
    public async Task<ActionResult<ResponseMessage<UserDto>>> CreateUser([FromBody] CreateUserDto userDto)
    {
      try
      {
        // Simple validation
        var validationErrors = ValidateCreateUser(userDto);
        if (validationErrors.Any())
        {
          var validationResponse = ResponseMessage<UserDto>.ValidationError("Invalid input data", validationErrors);
          return BadRequest(validationResponse);
        }

        var user = await userService.CreateUserAsync(userDto);
        if (user is null)
        {
          var conflictResponse = ResponseMessage<UserDto>.Conflict("User with this email already exists");
          return Conflict(conflictResponse);
        }

        var response = ResponseMessage<UserDto>.SuccessResult(user, "Usuario creado correctamente");
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<UserDto>.Error($"Error creating user: {ex.Message}", "USER_CREATION_ERROR");
        return StatusCode(500, response);
      }
    }

    /// <summary>
    /// Update an existing user (partial update)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="userDto">User update data</param>
    /// <returns>Updated user information</returns>
    [HttpPut("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResponseMessage<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
    {
      try
      {
        // Check if at least one field is provided
        if (!HasAtLeastOneField(userDto))
        {
          var validationResponse = ResponseMessage<UserDto>.ValidationError(
              "At least one field must be provided for update",
              new List<ValidationError>
              {
                new ValidationError("Request", "Please provide at least one field to update (Name, Surname, Email, or RoleId)")
              }
          );
          return BadRequest(validationResponse);
        }

        // Simple validation for provided fields
        var validationErrors = ValidateUpdateUser(userDto);
        if (validationErrors.Any())
        {
          var validationResponse = ResponseMessage<UserDto>.ValidationError("Invalid input data", validationErrors);
          return BadRequest(validationResponse);
        }

        var user = await userService.UpdateUserAsync(id, userDto);
        if (user == null)
        {
          var notFoundResponse = ResponseMessage<UserDto>.NotFound($"User with ID {id} not found");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage<UserDto>.SuccessResult(user, "User updated successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<UserDto>.Error($"Error updating user: {ex.Message}", "USER_UPDATE_ERROR");
        return StatusCode(500, response);
      }
    }

    /// <summary>
    /// Update user profile (self-service for users to update their own profile)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="profileDto">Profile update data</param>
    /// <returns>Updated user information</returns>
    [HttpPut("{id}/profile")]
    public async Task<ActionResult<ResponseMessage<UserDto>>> UpdateProfile(int id, [FromBody] UpdateProfileDto profileDto)
    {
      try
      {
        var user = await userService.UpdateProfileAsync(id, profileDto);
        if (user == null)
        {
          var notFoundResponse = ResponseMessage<UserDto>.NotFound($"User with ID {id} not found");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage<UserDto>.SuccessResult(user, "Profile updated successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<UserDto>.Error($"Error updating profile: {ex.Message}", "PROFILE_UPDATE_ERROR");
        return StatusCode(500, response);
      }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResponseMessage>> DeleteUser(int id)
    {
      try
      {
        var result = await userService.DeleteUserAsync(id);
        if (!result)
        {
          var notFoundResponse = ResponseMessage.NotFound($"User with ID {id} not found or could not be deleted");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage.SuccessMessage("User deleted successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage.Error($"Error deleting user: {ex.Message}", "USER_DELETION_ERROR");
        return StatusCode(500, response);
      }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="changePasswordDto">Password change data</param>
    /// <returns>Success confirmation</returns>
    [HttpPatch("{id}/change-password")]
    public async Task<ActionResult<ResponseMessage>> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
    {
      try
      {
        // Simple validation
        var validationErrors = ValidateChangePassword(changePasswordDto);
        if (validationErrors.Any())
        {
          var validationResponse = ResponseMessage.ValidationError("Invalid password data", validationErrors);
          return BadRequest(validationResponse);
        }

        var result = await userService.ChangePasswordAsync(id, changePasswordDto);
        if (!result)
        {
          var notFoundResponse = ResponseMessage.NotFound($"Contraseña incorrecta");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage.SuccessMessage("Contraseña actualizada correctamente");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage.Error($"Error changing password: {ex.Message}", "PASSWORD_CHANGE_ERROR");
        return StatusCode(500, response);
      }
    }

    /// <summary>
    /// Update user status (activate or deactivate)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="statusDto">Status update data</param>
    /// <returns>Success confirmation</returns>
    [HttpPatch("{id}/status")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResponseMessage>> UpdateUserStatus(int id, [FromBody] UpdateUserStatusDto statusDto)
    {
      try
      {
        // Validate DTO
        if (statusDto == null)
        {
          var validationResponse = ResponseMessage.ValidationError(
            "Invalid request data",
            new List<ValidationError>
            {
              new ValidationError("Request", "Status update data is required")
            }
          );
          return BadRequest(validationResponse);
        }

        // Validate statusId
        if (statusDto.StatusId != UserStatusIds.Active && statusDto.StatusId != UserStatusIds.Inactive)
        {
          var validationResponse = ResponseMessage.ValidationError(
            "Invalid status ID",
            new List<ValidationError>
            {
              new ValidationError("StatusId", "StatusId must be 1 (Active) or 2 (Inactive)")
            }
          );
          return BadRequest(validationResponse);
        }

        var result = await userService.UpdateUserStatusAsync(id, statusDto.StatusId);
        if (!result)
        {
          var notFoundResponse = ResponseMessage.NotFound($"User with ID {id} not found or could not be updated");
          return NotFound(notFoundResponse);
        }

        var statusName = statusDto.StatusId == UserStatusIds.Active ? "activated" : "deactivated";
        var response = ResponseMessage.SuccessMessage($"User {statusName} successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage.Error($"Error updating user status: {ex.Message}", "USER_STATUS_UPDATE_ERROR");
        return StatusCode(500, response);
      }
    }

    /// <summary>
    /// Get user roles
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User role information</returns>
    [HttpGet("{id}/roles")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResponseMessage<RolComposite>>> GetUserRoles(int id)
    {
      try
      {
        var role = await userService.GetUserRoleAsync(id);
        if (role == null)
        {
          var notFoundResponse = ResponseMessage<RolComposite>.NotFound($"User with ID {id} not found or has no assigned role");
          return NotFound(notFoundResponse);
        }

        var response = ResponseMessage<RolComposite>.SuccessResult(role, "User role retrieved successfully");
        return Ok(response);
      }
      catch (Exception ex)
      {
        var response = ResponseMessage<RolComposite>.Error($"Error retrieving user role: {ex.Message}", "USER_ROLE_RETRIEVAL_ERROR");
        return StatusCode(500, response);
      }
    }

        [Authorize]
        [HttpGet("players")]

        public async Task<ActionResult<IEnumerable<UserDto>>> SearchPlayersByEmail([FromQuery] string email)
        {
            try
            {
                var players = await userService.SearchPlayersByEmailAsync(email);
                return Ok(players);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar jugadores: {ex.Message}");
            }
        }

        #region Simple Validation Methods

        /// <summary>
        /// Simple validation for creating a user
        /// </summary>
        private List<ValidationError> ValidateCreateUser(CreateUserDto userDto)
    {
      var errors = new List<ValidationError>();

      if (string.IsNullOrWhiteSpace(userDto.Name))
        errors.Add(new ValidationError("Name", "Name is required"));

      if (string.IsNullOrWhiteSpace(userDto.Surname))
        errors.Add(new ValidationError("Surname", "Surname is required"));

      if (string.IsNullOrWhiteSpace(userDto.Email))
        errors.Add(new ValidationError("Email", "Email is required"));
      else if (!ValidationHelper.IsValidEmail(userDto.Email))
        errors.Add(new ValidationError("Email", "Invalid email format"));

      if (userDto.RoleId <= 0)
        errors.Add(new ValidationError("RoleId", "RoleId must be positive"));

      return errors;
    }

    /// <summary>
    /// Simple validation for updating a user
    /// </summary>
    private List<ValidationError> ValidateUpdateUser(UpdateUserDto userDto)
    {
      var errors = new List<ValidationError>();

      // Only validate fields that are provided
      if (!string.IsNullOrWhiteSpace(userDto.Name))
      {
        if (userDto.Name.Length > 100)
          errors.Add(new ValidationError("Name", "Name cannot exceed 100 characters"));
      }

      if (!string.IsNullOrWhiteSpace(userDto.Surname))
      {
        if (userDto.Surname.Length > 100)
          errors.Add(new ValidationError("Surname", "Surname cannot exceed 100 characters"));
      }

      if (!string.IsNullOrWhiteSpace(userDto.Email))
      {
        if (!ValidationHelper.IsValidEmail(userDto.Email))
          errors.Add(new ValidationError("Email", "Invalid email format"));
      }

      if (userDto.RoleId.HasValue && userDto.RoleId.Value <= 0)
      {
        errors.Add(new ValidationError("RoleId", "RoleId must be positive"));
      }

      return errors;
    }

    /// <summary>
    /// Simple validation for changing password
    /// </summary>
    private List<ValidationError> ValidateChangePassword(ChangePasswordDto changePasswordDto)
    {
      var errors = new List<ValidationError>();

      if (string.IsNullOrWhiteSpace(changePasswordDto.OldPassword))
        errors.Add(new ValidationError("OldPassword", "Old password is required"));

      if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
        errors.Add(new ValidationError("NewPassword", "New password is required"));
      else if (changePasswordDto.NewPassword.Length < 6)
        errors.Add(new ValidationError("NewPassword", "New password must be at least 6 characters"));

      return errors;
    }

    /// <summary>
    /// Check if at least one field is provided for partial updates
    /// </summary>
    private bool HasAtLeastOneField(UpdateUserDto userDto)
    {
      return !string.IsNullOrWhiteSpace(userDto.Name) ||
             !string.IsNullOrWhiteSpace(userDto.Surname) ||
             !string.IsNullOrWhiteSpace(userDto.Email) ||
             userDto.RoleId.HasValue;
    }

    #endregion
  }
}

