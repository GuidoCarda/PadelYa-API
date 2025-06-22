using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.User;
using padelya_api.Models;

namespace padelya_api.Services
{
    public class UserService(PadelYaDbContext context, IConfiguration configuration) : IUserService
    {
        private readonly PadelYaDbContext _context = context;

        public async Task<IEnumerable<UserDto>> GetUsersAsync(string? search = null, int? statusId = null)
        {
            var query = _context.Users
                .Include(u => u.Status)
                .Include(u => u.Role)
                .Include(u => u.Person)
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
                    Name = p.Name,
                    Surname = p.Surname,
                    PersonType = "Player",
                    Category = p.Category,
                    PreferredPosition = p.PreferredPosition
                },
                Teacher t => new TeacherDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Surname = t.Surname,
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

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
               .HashPassword(user, request.Password);

            user.Email = request.Email;
            user.PasswordHash = hashedPassword;

            var role = await _context.RolComposites.FindAsync(user.RoleId);

            if (role == null)
            {
                return null;
            }

            user.RoleId = request.RoleId;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                StatusId = user.StatusId,
                StatusName = user.Status.Name,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
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

            var role = await _context.RolComposites.FindAsync(userDto.RoleId);

            if (role == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(userDto.Email))
                user.Email = userDto.Email;

            if (userDto.RoleId.HasValue)
                user.RoleId = userDto.RoleId.Value;

            if (user.Person != null)
            {
                if (!string.IsNullOrEmpty(userDto.Name))
                {
                    user.Person.Name = userDto.Name;
                }

                if (!string.IsNullOrEmpty(userDto.Surname))
                {
                    user.Person.Surname = userDto.Surname;
                }
            }

            await _context.SaveChangesAsync();
            return new UserDto
            {
                Id = user.Id,
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
    }


}
