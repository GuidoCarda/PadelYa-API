using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using padelya_api.DTOs;
using padelya_api.Models;
using padelya_api.Services;

namespace padelya_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);

            if (user is null)
            {
                return BadRequest("El usuario ya existe");
            }
            return Ok(user);
        }

        [HttpPost("register-player")]
        public async Task<ActionResult<User>> RegisterPlayer(PlayerRegisterDto request)
        {
            var user = await authService.RegisterPlayerAsync(request);

            if (user is null)
            {
                return BadRequest("El usuario ya existe");
            }
            return Ok(user);
        }

        [HttpPost("register-teacher")]
        public async Task<IActionResult> RegisterTeacher(TeacherRegisterDto dto)
        {
            var user = await authService.RegisterTeacherAsync(dto);
            if (user is null)
            {
                return BadRequest("El usuario ya existe");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);

            if (result is null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(result);
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);

            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            return Ok(result);
        }


        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are authenticated as an admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-user")]
        public async Task<ActionResult<User>> CreateUser(CreateUserDto request)
        {
            var user = await authService.CreateUserAsync(request);

            if (user is null)
            {
                return BadRequest("El usuario ya existe");
            }
            return Ok(user);
        }

    }
}
