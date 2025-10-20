
using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Repair;
using padelya_api.Models;
using padelya_api.Models.Repair;
using padelya_api.Models.Repair.States;

namespace padelya_api.Services
{
  public class RepairService : IRepairService
  {
    private readonly PadelYaDbContext _context;
    private readonly ICourtSlotService _courtSlotService;
    private readonly IConfiguration _configuration;

    public RepairService(PadelYaDbContext context, ICourtSlotService courtSlotService, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
      _courtSlotService = courtSlotService;
    }

    public async Task<IEnumerable<Repair>> GetAllAsync(
      string? email = null,
      string? status = null,
      string? startDate = null,
      string? endDate = null)
    {

      var query = _context.Repairs
        .Include(r => r.Racket)
        .Include(r => r.Payment)
        .Include(r => r.Person)
        .AsQueryable();

      if (!string.IsNullOrEmpty(email))
      {
        var userEmails = _context.Users
            .Where(u => u.Email.Contains(email))
            .Select(u => u.PersonId)
            .ToList();

        query = query.Where(r => userEmails.Contains(r.PersonId));
      }

      if (!string.IsNullOrEmpty(startDate))
      {
        if (DateTime.TryParseExact(startDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var start))
        {
          query = query.Where(r => r.CreatedAt >= start.Date);
        }
        else
        {
          throw new ArgumentException("El formato de fecha debe ser YYYY-MM-DD");
        }

      }

      if (!string.IsNullOrEmpty(endDate))
      {
        if (DateTime.TryParseExact(endDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var end))
        {
          query = query.Where(r => r.CreatedAt <= end.Date);
        }
        else
        {
          throw new ArgumentException("El formato de fecha debe ser YYYY-MM-DD");
        }
      }

      if (!string.IsNullOrEmpty(status))
      {
        if (Enum.TryParse<RepairStatus>(status, true, out var statusEnum))
        {
          query = query.Where(r => r.Status == statusEnum);
        }
      }

      var repairs = await query.ToListAsync();

      return repairs.Select(r => new Repair
      {
        Id = r.Id,
        Racket = r.Racket,
        Person = r.Person,
        Payment = r.Payment,
        Price = r.Price,
        DamageDescription = r.DamageDescription,
        RepairNotes = r.RepairNotes,
        Status = r.Status,
        CreatedAt = r.CreatedAt,
        FinishedAt = r.FinishedAt,
        DeliveredAt = r.DeliveredAt,
        EstimatedCompletionTime = r.EstimatedCompletionTime,
        CustomerName = r.CustomerName,
        CustomerEmail = r.CustomerEmail,
        CustomerPhone = r.CustomerPhone,
      }).ToList();
    }


    public async Task<Repair?> GetByIdAsync(int id)
    {
      return await _context.Repairs
        .Include(r => r.Racket)
        .Include(r => r.Payment)
        .Include(r => r.Person)
        .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Repair> CreateAsync(CreateRepairDto dto)
    {
      Person? person;

      if (dto.PersonId != null)
      {
        person = await _context.Persons.FindAsync(dto.PersonId.Value);
        if (person == null)
          throw new Exception("Person not found.");
      }
      else
      {
        person = await _context.Persons
          .FirstOrDefaultAsync(p => p.Email == dto.CustomerEmail
          && p.PhoneNumber == dto.CustomerPhone);

        if (person == null)
        {
          person = new Player
          {
            Email = dto.CustomerEmail,
            PhoneNumber = dto.CustomerPhone,
            Name = dto.CustomerName.Split(' ')[0],
            Surname = dto.CustomerName.Split(' ')[1],
            Birthdate = DateTime.MinValue,
          };

          _context.Persons.Add(person);
          await _context.SaveChangesAsync();
        }
      }

      var racket = new Racket
      {
        Brand = dto.Racket.Brand,
        Model = dto.Racket.Model,
        SerialCode = dto.Racket.SerialCode,
      };

      _context.Rackets.Add(racket);
      await _context.SaveChangesAsync();

      // Create repair
      var repair = new Repair
      {
        PersonId = person.Id,
        RacketId = racket.Id,
        CustomerName = dto.CustomerName,
        CustomerEmail = person.Email,
        CustomerPhone = person.PhoneNumber,
        Price = dto.Price,
        DamageDescription = dto.DamageDescription,
        EstimatedCompletionTime = dto.EstimatedCompletionTime,
        CreatedAt = DateTime.UtcNow,
        Status = RepairStatus.Received,
      };

      _context.Repairs.Add(repair);
      await _context.SaveChangesAsync();

      repair.InitializeState(); // Initialize state pattern
      return repair;
    }

    public async Task<Repair> UpdateAsync(int id, UpdateRepairDto dto)
    {
      var repair = await _context.Repairs
        .Include(r => r.Racket)
        .Include(r => r.Person)
        .FirstOrDefaultAsync(r => r.Id == id);

      if (repair == null)
        throw new KeyNotFoundException("Repair not found");

      repair.InitializeState();

      // is this ok?
      repair.CustomerName = dto.CustomerName ?? repair.CustomerName;
      repair.CustomerEmail = dto.CustomerEmail ?? repair.CustomerEmail;
      repair.CustomerPhone = dto.CustomerPhone ?? repair.CustomerPhone;
      repair.Racket.Brand = dto.Racket?.Brand ?? repair.Racket.Brand;
      repair.Racket.Model = dto.Racket?.Model ?? repair.Racket.Model;
      repair.Racket.SerialCode = dto.Racket?.SerialCode ?? repair.Racket.SerialCode;
      repair.Price = dto.Price ?? repair.Price;
      repair.DamageDescription = dto.DamageDescription ?? repair.DamageDescription;
      repair.RepairNotes = dto.RepairNotes ?? repair.RepairNotes;

      await _context.SaveChangesAsync();
      return repair;
    }


    public async Task<Repair> CancelAsync(int repairId)
    {
      // 1. Get repair from DB
      var repair = await _context.Repairs
          .Include(r => r.Racket)
          .Include(r => r.Person)
          .FirstOrDefaultAsync(r => r.Id == repairId);

      if (repair == null)
        throw new KeyNotFoundException("Repair not found");

      // 2. Initialize state from DB status
      repair.InitializeState();

      try
      {
        repair.CancelRepair();

        repair.State.NotifyCustomer(repair.Racket);

        await _context.SaveChangesAsync();

        return repair;
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException(
            $"Cannot cancel repair in {repair.GetCurrentStatus()} state", ex);
      }
    }
  }
}