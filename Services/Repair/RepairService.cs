
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

      repair.InitializeState();
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

      // Initialize state for validation
      repair.InitializeState();

      // Only update fields that are provided (not null)
      // Customer fields - only update if no linked person (walk-in client)
      if (repair.Person == null || !_context.Users.Any(u => u.PersonId == repair.PersonId))
      {
        if (dto.CustomerName != null)
          repair.CustomerName = dto.CustomerName;
        if (dto.CustomerEmail != null)
          repair.CustomerEmail = dto.CustomerEmail;
        if (dto.CustomerPhone != null)
          repair.CustomerPhone = dto.CustomerPhone;
      }

      // Racket details
      if (dto.Racket != null)
      {
        if (dto.Racket.Brand != null)
          repair.Racket.Brand = dto.Racket.Brand;
        if (dto.Racket.Model != null)
          repair.Racket.Model = dto.Racket.Model;
        if (dto.Racket.SerialCode != null)
          repair.Racket.SerialCode = dto.Racket.SerialCode;
      }

      // Repair details
      if (dto.Price.HasValue)
        repair.Price = dto.Price.Value;
      if (dto.DamageDescription != null)
        repair.DamageDescription = dto.DamageDescription;
      if (dto.RepairNotes != null)
        repair.RepairNotes = dto.RepairNotes;
      if (dto.EstimatedCompletionTime.HasValue)
        repair.EstimatedCompletionTime = dto.EstimatedCompletionTime.Value;

      // Handle status transition with state pattern
      if (dto.Status != null && !string.IsNullOrEmpty(dto.Status))
      {
        if (Enum.TryParse<RepairStatus>(dto.Status, true, out var newStatus))
        {
          // If changing to cancelled, use the CancelRepair method
          if (newStatus == RepairStatus.Cancelled)
          {
            try
            {
              repair.CancelRepair();
              repair.Status = RepairStatus.Cancelled;
            }
            catch (InvalidOperationException ex)
            {
              throw new InvalidOperationException(
                $"Cannot cancel repair in {repair.GetCurrentStatus()} state", ex);
            }
          }
          // For advancing through normal states
          else if (newStatus != repair.Status)
          {
            // Validate it's a valid transition by attempting it
            var validTransitions = GetValidStatusTransitions(repair.Status);
            if (!validTransitions.Contains(newStatus))
            {
              throw new InvalidOperationException(
                $"Invalid status transition from {repair.Status} to {newStatus}");
            }

            // Advance state and update status enum
            repair.AdvanceRepairProcess();
            repair.Status = newStatus;

            // Set timestamps based on status
            if (newStatus == RepairStatus.ReadyForPickup)
              repair.FinishedAt = DateTime.UtcNow;
            else if (newStatus == RepairStatus.Delivered)
              repair.DeliveredAt = DateTime.UtcNow;
          }
        }
        else
        {
          throw new ArgumentException($"Invalid status value: {dto.Status}");
        }
      }

      await _context.SaveChangesAsync();
      return repair;
    }

    // Helper method to get valid status transitions
    private List<RepairStatus> GetValidStatusTransitions(RepairStatus currentStatus)
    {
      return currentStatus switch
      {
        RepairStatus.Received => new List<RepairStatus> { RepairStatus.InRepair, RepairStatus.Cancelled },
        RepairStatus.InRepair => new List<RepairStatus> { RepairStatus.ReadyForPickup, RepairStatus.Cancelled },
        RepairStatus.ReadyForPickup => new List<RepairStatus> { RepairStatus.Delivered },
        RepairStatus.Delivered => new List<RepairStatus>(),
        RepairStatus.Cancelled => new List<RepairStatus>(),
        _ => new List<RepairStatus>()
      };
    }


    public async Task<Repair> CancelAsync(int repairId)
    {
      var repair = await _context.Repairs
          .Include(r => r.Racket)
          .Include(r => r.Person)
          .FirstOrDefaultAsync(r => r.Id == repairId);

      if (repair == null)
      {
        throw new KeyNotFoundException("Repair not found");
      }

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

    public async Task<Repair> UpdateStatusAsync(int id, UpdateStatusDto dto)
    {
      var repair = await _context.Repairs.FindAsync(id);

      if (repair is null)
      {
        throw new KeyNotFoundException("Repair not found");
      }

      if (string.IsNullOrWhiteSpace(dto.Status))
      {
        throw new ArgumentException("Status is required");
      }

      if (!Enum.TryParse<RepairStatus>(dto.Status, true, out var newStatus))
      {
        throw new ArgumentException("Invalid status value");
      }

      repair.InitializeState();

      if (newStatus == RepairStatus.Cancelled)
      {
        repair.CancelRepair();
        await _context.SaveChangesAsync();
      }

      if (newStatus == repair.Status)
        return repair; // idempotent operation

      var validTransitions = GetValidStatusTransitions(repair.Status);
      if (!validTransitions.Contains(newStatus))
      {
        throw new InvalidOperationException($"Invalid status transition from {repair.Status} to {newStatus}");
      }

      repair.AdvanceRepairProcess();
      await _context.SaveChangesAsync();
      return repair;
    }


    public async Task<Repair> RegisterPaymentAsync(int id, RegisterRepairPaymentDto dto)
    {
      var repair = await _context.Repairs
        .Include(r => r.Racket)
        .Include(r => r.Person)
        .FirstOrDefaultAsync(r => r.Id == id);

      if (repair is null)
      {
        throw new KeyNotFoundException("Repair not found");
      }

      if (repair.Status != RepairStatus.ReadyForPickup)
      {
        throw new InvalidOperationException("Repair is not ready for pickup");
      }

      if (repair.PaymentId.HasValue)
        throw new InvalidOperationException("Payment already registered for this repair");

      var method = (dto.PaymentMethod ?? "").Trim().ToLowerInvariant();
      if (method != "cash" && method != "bank")
        throw new ArgumentException("Invalid payment method. Allowed: cash, bank");

      var payment = new Payment
      {
        Amount = repair.Price,
        PaymentMethod = method,
        PaymentStatus = Constants.PaymentStatus.Approved,
        PaymentType = Constants.PaymentType.Total,
        CreatedAt = DateTime.UtcNow,
        TransactionId = Guid.NewGuid().ToString("N"),
        PersonId = repair.PersonId
      };

      _context.Payments.Add(payment);
      await _context.SaveChangesAsync();

      repair.PaymentId = payment.Id;
      repair.Payment = payment;

      // Advance to Delivered as part of pickup
      repair.InitializeState();
      repair.AdvanceRepairProcess();

      await _context.SaveChangesAsync();
      return repair;
    }
  }
}
