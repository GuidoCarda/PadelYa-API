using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using padelya_api.Data;
using padelya_api.DTOs;
using padelya_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace padelya_api.Services
{
    public class AuthService(PadelYaDbContext context, IConfiguration configuration) : IAuthService
    {
        private readonly PadelYaDbContext _context = context;

        public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
        {

            var user = await _context.Users
                .Include(u => u.Person)
                .Include(u => u.Role)
                    .ThenInclude(r=>r.Permissions)
                    .ThenInclude(p => (p as SimplePermission).Form)
                .FirstOrDefaultAsync(u => u.Email == request.Email);
                
            if (user is null)
            {
                return null;
            }

            if (user.Email != request.Email)
            {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var tokenResponse = await CreateTokenResponse(user);
            tokenResponse.Person = user.Person;
            return  tokenResponse;
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {

            var permissions = new HashSet<string>();
            var forms = new HashSet<string>();

            GetPermissionsAndForms(user.Role, permissions, forms);

            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
                Permissions = permissions.ToList(),
                Forms = forms.ToList()
            };
        }

        async Task<TokenResponseDto?> IAuthService.RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var userIdGuid = Guid.Parse(request.UserId.ToString()); // Convert 'int' to 'Guid'
            var user = await ValidateRefreshTokenAsync(userIdGuid, request.RefreshToken);
            
            if (user is null)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }

            var user = new User();

            var hashedPassword = new PasswordHasher<User>()
               .HashPassword(user, request.Password);

            user.Email = request.Email;
            user.PasswordHash = hashedPassword;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
                
        public async Task<User?> RegisterPlayerAsync(PlayerRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }


            var player = new Player()
            {
                Name = request.Name,
                Surname = request.Surname,
                Birthdate = request.Birthdate,
                Category = request.Category,
                PreferredPosition = request.PreferredPosition
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
               .HashPassword(user, request.Password);

            user.Email = request.Email;
            user.PasswordHash = hashedPassword;
            user.RoleId = 102;
            user.UserType = "Player";
            user.PersonId = player.Id;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> RegisterTeacherAsync(TeacherRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }

            var teacher = new Teacher()
            {
                Name = request.Name,
                Surname = request.Surname,
                Birthdate = request.Birthdate,
                Category = request.Category,
                Institution = request.Institution,
                Title = request.Title
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
               .HashPassword(user, request.Password);

            user.Email = request.Email;
            user.PasswordHash = hashedPassword;
            user.RoleId = 101;
            user.UserType = "Teacher";
            user.PersonId = teacher.Id;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
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

        private void GetPermissionsAndForms(PermissionComponent component, HashSet<string> permissions, HashSet<string> forms)
        {
            if (component is SimplePermission simple)
            {
                permissions.Add(simple.Name);
                if (simple.Form != null)
                    forms.Add(simple.Form.Name);
            }
            else if (component is RolComposite composite)
            {
                foreach (var perm in composite.Permissions)
                {
                    GetPermissionsAndForms(perm, permissions, forms);
                }
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
           {
               new Claim("email", user.Email),
               new Claim("user_id", user.Id.ToString()),
               new Claim("role", user.Role.Name)
           };

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                throw new Exception("No existe un usuario con ese mail.");
            }

            var newPassword = GenerateRandomPassword();

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, newPassword);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Recovery email sent to {email}");
            return true;
        }


        private static string GenerateRandomPassword(int length = 12)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            StringBuilder res = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (res.Length < length)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }
            return res.ToString();
        }
    }
}
