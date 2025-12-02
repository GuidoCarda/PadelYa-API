using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Auth;
using padelya_api.DTOs.User;
using padelya_api.Services;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
  [Route("api/auth")]
  [ApiController]
  public class AuthController(IAuthService authService) : ControllerBase
  {

    [HttpPost("register-player")]
    public async Task<ActionResult<TokenResponseDto>> RegisterPlayer(RegisterPlayerDto request)
    {
      var tokenResponse = await authService.RegisterPlayerAsync(request);

      if (tokenResponse is null)
      {
        return BadRequest("El usuario ya existe");
      }
      return Ok(tokenResponse);
    }

    [HttpPost("register-teacher")]
    public async Task<ActionResult<TokenResponseDto>> RegisterTeacher(RegisterTeacherDto dto)
    {
      var tokenResponse = await authService.RegisterTeacherAsync(dto);
      if (tokenResponse is null)
      {
        return BadRequest("El usuario ya existe");
      }
      return Ok(tokenResponse);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserLoginDto request)
    {
      var result = await authService.LoginAsync(request);

      if (result is null)
      {
        return BadRequest("Correo o contraseña invalidos");
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

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
      var userIdClaim = User.FindFirst("user_id")?.Value;

      if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
      {
        return Unauthorized("Invalid user");
      }

      await authService.LogoutAsync(userId);

      return Ok(new { message = "Logout successful" });
    }


    // [Authorize]
    [HttpGet]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
      return Ok("You are authenticated");
    }

    // [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnlyEndpoint()
    {
      return Ok("You are authenticated as an admin");
    }

    [HttpPost("recover-password")]
    public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordDto request)
    {
      try
      {
        var result = await authService.RecoverPasswordAsync(request.Email);
        return Ok(ResponseMessage<RecoverPasswordDto>.SuccessResult(request, "Recuperación de contraseña iniciada. Por favor revisa tu correo."));    
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<RecoverPasswordDto>.Error($"No se pudo iniciar la recuperación de contraseña. Verifica que el correo sea correcto: {ex.Message}"));
      }
    }
  }
}