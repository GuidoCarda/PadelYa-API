using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs;
using padelya_api.Models;

namespace padelya_api.Services
{
  public class UserService(PadelYaDbContext context, IConfiguration configuration) : IUserService
  {
    private readonly PadelYaDbContext _context = context;

        public async Task<IEnumerable<User>> GetUsersAsync(string? search = null, int? statusId = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
            }

            if (statusId.HasValue)
            {
                query = query.Where(u => u.StatusId == statusId);
            }

            return await query.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User?> CreateUserAsync(CreateUserDto request)
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
            user.RoleId = request.RoleId;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                return null; 
            }

            user.Name = userDto.Name;
            user.Surname = userDto.Surname;
            user.Email = userDto.Email;
            user.RoleId = userDto.RoleId;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null)
            {
                return false;
            }

            if(string.IsNullOrEmpty(changePasswordDto.NewPassword) || changePasswordDto.NewPassword.Length < 6)
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

            if(user == null)
            {
                return false; // User not found
            }
            // check the status Id
            user.StatusId = 2;
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
