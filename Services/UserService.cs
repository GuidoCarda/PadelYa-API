using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.User;
using padelya_api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace padelya_api.Services
{
    public class UserService(PadelYaDbContext context, IPasswordService passwordService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : IUserService
    {
        private readonly PadelYaDbContext _context = context;
        private readonly IPasswordService _passwordService = passwordService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


        public async Task<IEnumerable<UserDto>> GetUsersAsync(string? search = null, int? statusId = null)
        {
            var query = _context.Users
                .Include(u => u.Status)
                .Include(u => u.Role)
                .Include(u => u.Person)
                .Where(u => u.StatusId == UserStatusIds.Active)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Email.Contains(search));
            }

            if (statusId.HasValue)
            {
                query = query.Where(u => u.StatusId == statusId);
            }

            var users = await query.ToListAsync();

            return users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    StatusId = user.StatusId,
                    StatusName = user.Status.Name,
                    RoleId = user.RoleId,
                    RoleName = user.Role.Name,
                    Person = user.Person == null ? null : MapPersonToDto(user.Person)
                })
                .ToList();
        }

        private static PersonDto? MapPersonToDto(Person person)
        {
            Console.WriteLine(person);
            return person switch
            {
                Player p => new PlayerDto
                {
                    Id = p.Id,
                    PersonType = "Player",
                    Category = p.Category,
                    PreferredPosition = p.PreferredPosition
                },
                Teacher t => new TeacherDto
                {
                    Id = t.Id,
                    PersonType = "Teacher",
                    Title = t.Title,
                    Institution = t.Institution,
                    Category = t.Category
                },
                _ => null
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Person)
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                StatusId = user.StatusId,
                StatusName = user.Status.Name,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
                Person = user.Person == null ? null : MapPersonToDto(user.Person)
            };
        }

        public async Task<UserDto?> CreateUserAsync(CreateUserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }

            Person? person = null;
            if (request?.Person?.PersonType == "Teacher")
            {
                // Intentar castear a TeacherDto, si falla usar valores por defecto
                string title = "Profesor";
                string institution = "PadelYa";
                
                if (request.Person is TeacherDto teacherDto)
                {
                    title = teacherDto.Title ?? title;
                    institution = teacherDto.Institution ?? institution;
                }

                person = new Teacher
                {
                    Name = request.Name ?? string.Empty,
                    Surname = request.Surname ?? string.Empty,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber ?? string.Empty,
                    Birthdate = request.Person.Birthdate,
                    Category = request.Person.Category,
                    Institution = institution,
                    Title = title
                };
            }
            else if (request?.Person?.PersonType == "Player")
            {
                // Intentar castear a PlayerDto, si falla usar valores por defecto
                string preferredPosition = "Cualquiera";
                
                if (request.Person is PlayerDto playerDto)
                {
                    preferredPosition = playerDto.PreferredPosition ?? preferredPosition;
                }

                person = new Player
                {
                    Name = request.Name ?? string.Empty,
                    Surname = request.Surname ?? string.Empty,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber ?? string.Empty,
                    Birthdate = request.Person.Birthdate,
                    Category = request.Person.Category,
                    PreferredPosition = preferredPosition
                };
            }

            var user = new User();

            var password = request.Password ?? string.Empty;
            if (string.IsNullOrEmpty(password))
            {
                // genero contraseña aleatoria
                password = _passwordService.GenerateRandomPassword();
                Console.WriteLine($"Contraseña generada {password}");
                Console.WriteLine($"Enviando contraseña al mail....  {request.Email}");
                // await _emailService.SendPasswordEmailAsync(request.Email, password);
                // TODO: implement emailService
            }

            var hashedPassword = _passwordService.HashPassword(user, password);

            var role = await _context.RolComposites.FindAsync(request.RoleId);
            if (role == null)
            {
                return null;
            }

            user.RoleId = request.RoleId;
            user.Email = request.Email;
            user.Name = request.Name ?? string.Empty;
            user.Surname = request.Surname ?? string.Empty;
            user.PasswordHash = hashedPassword;

            if (person != null)
            {
                _context.Add(person);
                await _context.SaveChangesAsync(); // Save to get the person ID
                user.PersonId = person.Id;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                StatusId = user.StatusId,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
                Person = person == null ? null : MapPersonToDto(person)
            };
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .Include(u => u.Role)
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            if (userDto.RoleId.HasValue)
            {
                var role = await _context.RolComposites.FindAsync(userDto.RoleId.Value);
                if (role == null)
                {
                    return null;
                }
                user.RoleId = userDto.RoleId.Value;
            }

            if (!string.IsNullOrEmpty(userDto.Email))
                user.Email = userDto.Email;

            if (!string.IsNullOrEmpty(userDto.Name))
                user.Name = userDto.Name;

            if (!string.IsNullOrEmpty(userDto.Surname))
                user.Surname = userDto.Surname;

            await _context.SaveChangesAsync();
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                StatusId = user.StatusId,
                StatusName = user.Status.Name,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
                Person = user.Person == null ? null : MapPersonToDto(user.Person)
            };
        }

        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(changePasswordDto.NewPassword) || changePasswordDto.NewPassword.Length < 6)
            {
                return false; // Invalid new password
            }

            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changePasswordDto.OldPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return false; // Old password does not match
            }

            var newHashedPassword = passwordHasher
                .HashPassword(user, changePasswordDto.NewPassword);

            user.PasswordHash = newHashedPassword;
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return false; // User not found
            }
            user.StatusId = UserStatusIds.Inactive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RolComposite?> GetUserRoleAsync(int userId)
        {
            var user = await _context.Users
              .Include(u => u.Role)
                  .ThenInclude(r => r.Permissions)
              .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Role;
        }

        public async Task<IEnumerable<UserDto>> SearchPlayersByEmailAsync(string email)
        {
            var loggedInUserIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUserId = int.Parse(loggedInUserIdString ?? "0");

            var playersQuery = _context.Users
                .Include(u => u.Person) 
                .Where(u => u.Email.ToLower().Contains(email.ToLower()))
                .Where(u => u.Id != loggedInUserId)
                .Where(u => u.PersonId != null);

            return await playersQuery
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,

                    // Creamos el objeto PersonDto (o PlayerDto) anidado
                    Person = u.Person != null ? new PlayerDto
                    {
                        Id = u.Person.Id,
                        PersonType = "player", 
                        Birthdate = u.Person.Birthdate,
                        Category = u.Person.Category,
                        // Hacemos un "cast" para acceder a las propiedades específicas de Player
                        PreferredPosition = (u.Person as Player) != null ? (u.Person as Player).PreferredPosition : null
                    } : null,

                    Permissions = new List<string>()
                })
                .ToListAsync();
        }

    }


}
