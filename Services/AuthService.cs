using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Auth;
using padelya_api.DTOs.User;
using padelya_api.Models;

namespace padelya_api.Services
{
    public class AuthService(
      PadelYaDbContext context,
      IPasswordService passwordService,
      IConfiguration configuration
      ) : IAuthService
    {
        private readonly PadelYaDbContext _context = context;
        private readonly IPasswordService _passwordService = passwordService;

        public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
        {

            var user = await _context.Users
                .Include(u => u.Person)
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                    .ThenInclude(p => (p as SimplePermission).Module)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.StatusId == UserStatusIds.Active);

            if (user is null)
            {
                return null;
            }

            if (user.Email != request.Email)
            {
                return null;
            }

            if (!_passwordService.VerifyPassword(user, user.PasswordHash, request.Password))
            {
                return null;
            }

            var tokenResponse = await CreateTokenResponse(user);
            tokenResponse.Person = user.Person;
            return tokenResponse;
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {

            var permissions = new HashSet<string>();
            var modules = new HashSet<string>();

            GetPermissionsAndModules(user.Role, permissions, modules);

            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
                Permissions = permissions.ToList(),
                Modules = modules.ToList()
            };
        }

        async Task<TokenResponseDto?> IAuthService.RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

            if (user is null)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                    .ThenInclude(p => (p as SimplePermission).Module)
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private async Task<TokenResponseDto?> RegisterUserAsync<TPerson>(
            string email,
            string password,
            int roleId,
            Func<TPerson> createPerson
        ) where TPerson : Person
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return null;
            }

            var person = createPerson();
            _context.Set<TPerson>().Add(person);
            await _context.SaveChangesAsync();


            var user = new User
            {
                Email = email,
                // PasswordHash = new PasswordHasher<User>().HashPassword(null, password),
                PasswordHash = _passwordService.HashPassword(null, password),
                RoleId = roleId,
                PersonId = person.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            var userWithRole = await _context.Users
             .Include(u => u.Role)
             .ThenInclude(r => r.Permissions)
             .FirstOrDefaultAsync(u => u.Id == user.Id);

            var tokenResponse = await CreateTokenResponse(userWithRole!);
            tokenResponse.Person = person;
            return tokenResponse;
        }

        public async Task<TokenResponseDto?> RegisterPlayerAsync(RegisterPlayerDto request)
        {
            return await RegisterUserAsync<Player>(
                request.Email,
                request.Password,
                102,
                () => new Player
                {
                    Name = request.Name,
                    Surname = request.Surname,
                    Birthdate = request.Birthdate,
                    Category = request.Category,
                    PreferredPosition = request.PreferredPosition
                }
                );
        }

        public async Task<TokenResponseDto?> RegisterTeacherAsync(RegisterTeacherDto request)
        {
            return await RegisterUserAsync<Teacher>(
                request.Email,
                request.Password,
                101,
                () => new Teacher
                {
                    Name = request.Name,
                    Surname = request.Surname,
                    Birthdate = request.Birthdate,
                    Category = request.Category,
                    Institution = request.Institution,
                    Title = request.Title
                });
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        private void GetPermissionsAndModules(PermissionComponent component, HashSet<string> permissions, HashSet<string> modules)
        {
            if (component is SimplePermission simple)
            {
                permissions.Add(simple.Name);
                if (simple.Module != null)
                    modules.Add(simple.Module.Name);
            }
            else if (component is RolComposite composite)
            {
                foreach (var perm in composite.Permissions)
                {
                    GetPermissionsAndModules(perm, permissions, modules);
                }
            }
        }

        private string CreateToken(User user)
        {
            var permissions = new HashSet<string>();
            var modules = new HashSet<string>();
            GetPermissionsAndModules(user.Role, permissions, modules);

            var claims = new List<Claim>
            {
                new Claim("email", user.Email),
                new Claim("user_id", user.Id.ToString()),
                new Claim("role", user.Role.Name)
            };

            // Add permissions as claims
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            // Add modules as claims
            foreach (var module in modules)
            {
                claims.Add(new Claim("module", module));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                    issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                    audience: configuration.GetValue<string>("AppSettings:Audience"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<bool> RecoverPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.StatusId == UserStatusIds.Active);
            if (user is null)
            {
                throw new Exception("No existe un usuario con ese mail.");
            }

            var newPassword = _passwordService.GenerateRandomPassword();

            user.PasswordHash = _passwordService.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Recovery email sent to {email}");
            return true;
        }



    }
}
