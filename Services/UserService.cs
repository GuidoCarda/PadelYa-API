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
  public class UserService(PadelYaDbContext context, IPasswordService passwordService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IBookingService bookingService) : IUserService
  {
    private readonly PadelYaDbContext _context = context;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IBookingService _bookingService = bookingService;


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

      // Get all person IDs for users that have a person
      var personIds = users.Where(u => u.PersonId.HasValue).Select(u => u.PersonId.Value).ToList();

      // Get booking counts from BookingService (separation of concerns)
      var bookingCountsDict = await _bookingService.GetBookingCountsByPersonIdsAsync(personIds);

      // Get all user IDs
      var userIds = users.Select(u => u.Id).ToList();

      // Get last login dates for all users in one query
      var lastLogins = await _context.LoginAudits
          .Where(la => userIds.Contains(la.UserId) && la.Action == LoginAuditAction.Login)
          .GroupBy(la => la.UserId)
          .Select(g => new { UserId = g.Key, LastLogin = g.Max(la => la.Timestamp) })
          .ToDictionaryAsync(x => x.UserId, x => x.LastLogin);

      return users
          .Select(user =>
          {
            var bookingCount = user.PersonId.HasValue && bookingCountsDict.ContainsKey(user.PersonId.Value)
                      ? bookingCountsDict[user.PersonId.Value]
                      : (ActiveCount: 0, TotalCount: 0);

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
              Person = user.Person == null ? null : MapPersonToDto(user.Person),
              Permissions = new List<string>(),
              Bookings = new UserBookingStatsDto
              {
                ActiveCount = bookingCount.ActiveCount,
                TotalCount = bookingCount.TotalCount
              },
              LastLoginAt = lastLogins.ContainsKey(user.Id) ? lastLogins[user.Id] : null,
              RegisteredAt = user.RegisteredAt
            };
          })
          .ToList();
    }

    private static PersonDto? MapPersonToDto(Person person)
    {
      return person switch
      {
        Player p => new PlayerDto
        {
          Id = p.Id,
          PersonType = "Player",
          Birthdate = p.Birthdate,
          Category = p.Category,
          PhoneNumber = p.PhoneNumber,
          PreferredPosition = p.PreferredPosition
        },
        Teacher t => new TeacherDto
        {
          Id = t.Id,
          PersonType = "Teacher",
          Birthdate = t.Birthdate,
          Title = t.Title,
          Institution = t.Institution,
          Category = t.Category,
          PhoneNumber = t.PhoneNumber
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

      // Get booking counts from BookingService
      UserBookingStatsDto? bookingStats = null;
      if (user.PersonId.HasValue)
      {
        var bookingCounts = await _bookingService.GetBookingCountsByPersonIdsAsync(new List<int> { user.PersonId.Value });
        if (bookingCounts.ContainsKey(user.PersonId.Value))
        {
          var counts = bookingCounts[user.PersonId.Value];
          bookingStats = new UserBookingStatsDto
          {
            ActiveCount = counts.ActiveCount,
            TotalCount = counts.TotalCount
          };
        }
      }

      // Get last login date
      var lastLogin = await _context.LoginAudits
          .Where(la => la.UserId == user.Id && la.Action == LoginAuditAction.Login)
          .OrderByDescending(la => la.Timestamp)
          .Select(la => la.Timestamp)
          .FirstOrDefaultAsync();

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
        Person = user.Person == null ? null : MapPersonToDto(user.Person),
        Permissions = new List<string>(),
        Bookings = bookingStats ?? new UserBookingStatsDto { ActiveCount = 0, TotalCount = 0 },
        LastLoginAt = lastLogin == default(DateTime) ? null : (DateTime?)lastLogin,
        RegisteredAt = user.RegisteredAt
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
          PhoneNumber = request.Person.PhoneNumber ?? string.Empty,
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
          PhoneNumber = request.Person.PhoneNumber ?? string.Empty,
          Birthdate = request.Person.Birthdate,
          Category = request.Person.Category,
          PreferredPosition = preferredPosition
        };
      }

      var user = new User
      {
        RegisteredAt = DateTime.UtcNow
      };

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
        Person = person == null ? null : MapPersonToDto(person),
        Permissions = new List<string>(),
        Bookings = new UserBookingStatsDto { ActiveCount = 0, TotalCount = 0 },
        LastLoginAt = null,
        RegisteredAt = user.RegisteredAt
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

      // Update Person.PhoneNumber if user has a Person and phoneNumber is provided
      if (user.Person != null && userDto.PhoneNumber != null)
      {
        user.Person.PhoneNumber = userDto.PhoneNumber;
      }

      await _context.SaveChangesAsync();

      // Get booking counts from BookingService
      UserBookingStatsDto? bookingStats = null;
      if (user.PersonId.HasValue)
      {
        var bookingCounts = await _bookingService.GetBookingCountsByPersonIdsAsync(new List<int> { user.PersonId.Value });
        if (bookingCounts.ContainsKey(user.PersonId.Value))
        {
          var counts = bookingCounts[user.PersonId.Value];
          bookingStats = new UserBookingStatsDto
          {
            ActiveCount = counts.ActiveCount,
            TotalCount = counts.TotalCount
          };
        }
      }

      // Get last login date
      var lastLogin = await _context.LoginAudits
          .Where(la => la.UserId == user.Id && la.Action == LoginAuditAction.Login)
          .OrderByDescending(la => la.Timestamp)
          .Select(la => la.Timestamp)
          .FirstOrDefaultAsync();

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
        Person = user.Person == null ? null : MapPersonToDto(user.Person),
        Permissions = new List<string>(),
        Bookings = bookingStats ?? new UserBookingStatsDto { ActiveCount = 0, TotalCount = 0 },
        LastLoginAt = lastLogin == default(DateTime) ? null : (DateTime?)lastLogin,
        RegisteredAt = user.RegisteredAt
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

    public async Task<bool> UpdateUserStatusAsync(int id, int statusId)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

      if (user == null)
      {
        return false; // User not found
      }

      // Validate statusId (should be 1 for Active or 2 for Inactive)
      if (statusId != UserStatusIds.Active && statusId != UserStatusIds.Inactive)
      {
        return false; // Invalid status ID
      }

      user.StatusId = statusId;
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<UserDto?> UpdateProfileAsync(int id, UpdateProfileDto profileDto)
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

      // Update basic user fields
      if (!string.IsNullOrEmpty(profileDto.Name))
        user.Name = profileDto.Name;

      if (!string.IsNullOrEmpty(profileDto.Surname))
        user.Surname = profileDto.Surname;

      // Update Person fields if user has a Person
      if (user.Person != null)
      {
        // Update Person name/surname to keep in sync
        if (!string.IsNullOrEmpty(profileDto.Name))
          user.Person.Name = profileDto.Name;

        if (!string.IsNullOrEmpty(profileDto.Surname))
          user.Person.Surname = profileDto.Surname;

        if (profileDto.PhoneNumber != null)
          user.Person.PhoneNumber = profileDto.PhoneNumber;

        if (profileDto.Birthdate.HasValue)
          user.Person.Birthdate = profileDto.Birthdate.Value;

        if (!string.IsNullOrEmpty(profileDto.Category))
          user.Person.Category = profileDto.Category;

        // Update Player-specific fields
        if (user.Person is Player player)
        {
          if (!string.IsNullOrEmpty(profileDto.PreferredPosition))
            player.PreferredPosition = profileDto.PreferredPosition;
        }

        // Update Teacher-specific fields
        if (user.Person is Teacher teacher)
        {
          if (!string.IsNullOrEmpty(profileDto.Title))
            teacher.Title = profileDto.Title;

          if (!string.IsNullOrEmpty(profileDto.Institution))
            teacher.Institution = profileDto.Institution;
        }
      }

      await _context.SaveChangesAsync();

      // Get booking counts from BookingService
      UserBookingStatsDto? bookingStats = null;
      if (user.PersonId.HasValue)
      {
        var bookingCounts = await _bookingService.GetBookingCountsByPersonIdsAsync(new List<int> { user.PersonId.Value });
        if (bookingCounts.ContainsKey(user.PersonId.Value))
        {
          var counts = bookingCounts[user.PersonId.Value];
          bookingStats = new UserBookingStatsDto
          {
            ActiveCount = counts.ActiveCount,
            TotalCount = counts.TotalCount
          };
        }
      }

      // Get last login date
      var lastLogin = await _context.LoginAudits
          .Where(la => la.UserId == user.Id && la.Action == LoginAuditAction.Login)
          .OrderByDescending(la => la.Timestamp)
          .Select(la => la.Timestamp)
          .FirstOrDefaultAsync();

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
        Person = user.Person == null ? null : MapPersonToDto(user.Person),
        Permissions = new List<string>(),
        Bookings = bookingStats ?? new UserBookingStatsDto { ActiveCount = 0, TotalCount = 0 },
        LastLoginAt = lastLogin == default(DateTime) ? null : (DateTime?)lastLogin,
        RegisteredAt = user.RegisteredAt
      };
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

      var players = await playersQuery.ToListAsync();
      var userIds = players.Select(u => u.Id).ToList();

      // Get last login dates for all users
      var lastLogins = userIds.Count > 0
          ? await _context.LoginAudits
              .Where(la => userIds.Contains(la.UserId) && la.Action == LoginAuditAction.Login)
              .GroupBy(la => la.UserId)
              .Select(g => new { UserId = g.Key, LastLogin = g.Max(la => la.Timestamp) })
              .ToDictionaryAsync(x => x.UserId, x => x.LastLogin)
          : new Dictionary<int, DateTime>();

      return players
          .Select(u => new UserDto
          {
            Id = u.Id,
            Name = u.Name,
            Surname = u.Surname,
            Email = u.Email,
            StatusId = u.StatusId,
            StatusName = u.Status?.Name ?? string.Empty,
            RoleId = u.RoleId,
            RoleName = u.Role?.Name ?? string.Empty,

            // Creamos el objeto PersonDto (o PlayerDto) anidado
            Person = u.Person != null ? new PlayerDto
            {
              Id = u.Person.Id,
              PersonType = "player",
              Birthdate = u.Person.Birthdate,
              Category = u.Person.Category,
              // Hacemos un "cast" para acceder a las propiedades específicas de Player
              PreferredPosition = (u.Person as Player)?.PreferredPosition ?? string.Empty
            } : null,

            Permissions = new List<string>(),
            Bookings = new UserBookingStatsDto { ActiveCount = 0, TotalCount = 0 },
            LastLoginAt = lastLogins.ContainsKey(u.Id) ? (DateTime?)lastLogins[u.Id] : null,
            RegisteredAt = u.RegisteredAt
          })
          .ToList();
    }

  }


}
