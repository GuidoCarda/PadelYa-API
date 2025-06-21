using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.User;
using padelya_api.Services;

namespace padelya_api.Controllers
{
    [Route("api/users")]
    [ApiController]
    //[Authorize] 
    public class UserController(IUserService userService, IRoleService roleService) : ControllerBase
    {
        // GET: api/users?search=...&statusId=...
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers(string? search = null, int? statusId = null)
        {
            var users = await userService.GetUsersAsync(search, statusId);
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }


        // POST: api/users
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            var user = await userService.CreateUserAsync(userDto);
            if (user is null)
            {
                return BadRequest("El usuario ya existe");
            }
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        //// PUT: api/users/{id}
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            var user = await userService.UpdateUserAsync(id, userDto);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        //// PATCH: api/users/{id}/activate
        //[HttpPatch("{id}/activate")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> ActivateUser(int id)
        //{
        //  var result = await userService.ActivateUserAsync(id);
        //  if (!result)
        //    return NotFound();
        //  return NoContent();
        //}

        // PATCH : api/users/{id}/change-password
        [HttpPatch("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await userService.ChangePasswordAsync(id, changePasswordDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        //// PATCH: api/users/{id}/deactivate
        //[HttpPatch("{id}/deactivate")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> DeactivateUser(int id)
        //{
        //  var result = await userService.DeactivateUserAsync(id);
        //  if (!result)
        //    return NotFound();
        //  return NoContent();
        //}

        // GET: api/users/{id}/roles
        [HttpGet("{id}/roles")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            var role = await userService.GetUserRoleAsync(id);
            return Ok(role);
        }

        //// POST: api/users/{id}/roles
        //[HttpPost("{id}/roles")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleDto assignRoleDto)
        //{
        //  var result = await userService.AssignRoleToUserAsync(id, assignRoleDto.RoleId);
        //  if (!result)
        //    return NotFound();
        //  return NoContent();
        //}
    }
}

