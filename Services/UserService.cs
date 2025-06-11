using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.Models;

namespace padelya_api.Services
{
  public class UserService(PadelYaDbContext context, IConfiguration configuration) : IUserService
  {
    private readonly PadelYaDbContext _context = context;


    public IEnumerable<User> GetUsers(string? search = null, int? statusId = null)
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

      return query.ToList();
    }
  }
}
