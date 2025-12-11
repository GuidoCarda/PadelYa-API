
using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Repair;
using padelya_api.Models;
using padelya_api.Models.Repair;
using padelya_api.Models.Repair.States;
using padelya_api.Services.Email;

namespace padelya_api.Services
{
  public class RepairService : IRepairService
  {
    private readonly PadelYaDbContext _context;
    private readonly ICourtSlotService _courtSlotService;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailNotificationService _emailNotificationService;

    public RepairService(
        PadelYaDbContext context,
        ICourtSlotService courtSlotService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        IEmailNotificationService emailNotificationService
    )
    {
      _context = context;
      _configuration = configuration;
      _courtSlotService = courtSlotService;
      _httpContextAccessor = httpContextAccessor;
      _emailNotificationService = emailNotificationService;
    }

    public async Task<IEnumerable<RepairResponseDto>> GetAllAsync(
      string? email = null,
      string? status = null,
      string? startDate = null,
      string? endDate = null)
    {

      var query = _context.Repairs
        .Include(r => r.Racket)
        .Include(r => r.Payment)
        .Include(r => r.Person)
        .Include(r => r.Audits)
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

      return repairs.Select(r => new RepairResponseDto
      {
        Id = r.Id,
        CreatedAt = r.CreatedAt,
        FinishedAt = r.FinishedAt,
        DeliveredAt = r.DeliveredAt,
        EstimatedCompletionTime = r.EstimatedCompletionTime,
        Price = r.Price,
        DamageDescription = r.DamageDescription,
        RepairNotes = r.RepairNotes,
        Status = r.Status,
        RacketId = r.RacketId,
        Racket = r.Racket,
        PersonId = r.PersonId,
        Person = r.Person,
        PaymentId = r.PaymentId,
        Payment = r.Payment,
        // Get cancellation reason from the Cancelled audit entry
        CancellationReason = r.Status == RepairStatus.Cancelled
            ? r.Audits.FirstOrDefault(a => a.Action == RepairAuditAction.Cancelled)?.Notes
            : null,
        // Build status history from audits
        StatusHistory = r.Audits
            .Where(a => a.NewStatus.HasValue)
            .OrderBy(a => a.Timestamp)
            .Select(a => new StatusHistoryDto
            {
              Status = a.NewStatus!.Value,
              Timestamp = a.Timestamp,
              Action = a.Action
            })
            .ToList()
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

    public async Task<IEnumerable<RepairResponseDto>> GetMyRepairsAsync()
    {
      var personId = GetCurrentPersonId();

      var repairs = await _context.Repairs
        .Include(r => r.Racket)
        .Include(r => r.Payment)
        .Include(r => r.Person)
        .Include(r => r.Audits)
        .Where(r => r.PersonId == personId)
        .OrderByDescending(r => r.CreatedAt)
        .ToListAsync();

      return repairs.Select(r => new RepairResponseDto
      {
        Id = r.Id,
        CreatedAt = r.CreatedAt,
        FinishedAt = r.FinishedAt,
        DeliveredAt = r.DeliveredAt,
        EstimatedCompletionTime = r.EstimatedCompletionTime,
        Price = r.Price,
        DamageDescription = r.DamageDescription,
        RepairNotes = r.RepairNotes,
        Status = r.Status,
        RacketId = r.RacketId,
        Racket = r.Racket,
        PersonId = r.PersonId,
        Person = r.Person,
        PaymentId = r.PaymentId,
        Payment = r.Payment,
        // Get cancellation reason from the Cancelled audit entry
        CancellationReason = r.Status == RepairStatus.Cancelled
            ? r.Audits.FirstOrDefault(a => a.Action == RepairAuditAction.Cancelled)?.Notes
            : null,
        // Build status history from audits
        StatusHistory = r.Audits
            .Where(a => a.NewStatus.HasValue)
            .OrderBy(a => a.Timestamp)
            .Select(a => new StatusHistoryDto
            {
              Status = a.NewStatus!.Value,
              Timestamp = a.Timestamp,
              Action = a.Action
            })
            .ToList()
      }).ToList();
    }

    private int GetCurrentPersonId()
    {
      var personIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("person_id");

      if (personIdClaim == null || !int.TryParse(personIdClaim.Value, out var personId))
      {
        throw new InvalidOperationException("Person ID not found in claims. User may not have a person profile.");
      }

      return personId;
    }

    public async Task<Repair> CreateAsync(CreateRepairDto dto)
    {
      // PersonId es obligatorio - solo se pueden crear reparaciones para usuarios existentes
      var person = await _context.Persons.FindAsync(dto.PersonId);
      if (person == null)
        throw new KeyNotFoundException("Person not found.");

      // Validaciones de negocio
      if (dto.Price <= 0)
        throw new ArgumentException("Price must be greater than zero.");

      if (string.IsNullOrWhiteSpace(dto.DamageDescription))
        throw new ArgumentException("Damage description is required.");

      if (dto.EstimatedCompletionTime < DateTime.UtcNow.Date)
        throw new ArgumentException("Estimated completion time cannot be in the past.");

      if (dto.Racket == null)
        throw new ArgumentException("Racket information is required.");

      if (string.IsNullOrWhiteSpace(dto.Racket.Brand))
        throw new ArgumentException("Racket brand is required.");

      if (string.IsNullOrWhiteSpace(dto.Racket.Model))
        throw new ArgumentException("Racket model is required.");

      if (string.IsNullOrWhiteSpace(dto.Racket.SerialCode))
        throw new ArgumentException("Racket serial code is required.");

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
        Price = dto.Price,
        DamageDescription = dto.DamageDescription,
        EstimatedCompletionTime = dto.EstimatedCompletionTime,
        CreatedAt = DateTime.UtcNow,
        Status = RepairStatus.Received,
      };

      _context.Repairs.Add(repair);
      await _context.SaveChangesAsync();

      LogRepairAuditAsync(repair.Id, RepairAuditAction.Created);

      await _context.SaveChangesAsync();

      // Cargar las relaciones para la respuesta
      repair.Person = person;
      repair.Racket = racket;

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
            var previousStatus = repair.Status;
            // Advance state and update status enum
            repair.AdvanceRepairProcess();
            repair.Status = newStatus;

            await NotifyCustomerStatusChangeAsync(repair, previousStatus);

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

      LogRepairAuditAsync(repair.Id, RepairAuditAction.Modified);

      if (dto.Status != null && Enum.TryParse<RepairStatus>(dto.Status.ToString(), true, out var newStatusEnum))
      {
        LogRepairAuditAsync(repair.Id, RepairAuditAction.StatusAdvanced, repair.Status, newStatusEnum);
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


    public async Task<Repair> CancelAsync(int repairId, CancelRepairDto cancellationDto)
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
        LogRepairAuditAsync(repair.Id, RepairAuditAction.Cancelled, reason: cancellationDto.Reason);
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
      var previousStatus = repair.Status;
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

      await NotifyCustomerStatusChangeAsync(repair, previousStatus);

      LogRepairAuditAsync(repair.Id, RepairAuditAction.StatusAdvanced, repair.Status, newStatus);
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


    private int GetCurrentUserId()
    {
      var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("user_id");

      if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
      {
        throw new InvalidOperationException("User ID not found in claims");
      }

      return userId;
    }

    private void LogRepairAuditAsync(
      int repairId,
      RepairAuditAction action,
      RepairStatus? oldStatus = null,
      RepairStatus? newStatus = null,
      string? reason = null
    )
    {

      var userId = GetCurrentUserId();

      _context.RepairAudits.Add(new RepairAudit
      {
        RepairId = repairId,
        Action = action,
        OldStatus = oldStatus,
        NewStatus = newStatus,
        Timestamp = DateTime.UtcNow,
        UserId = userId,
        Notes = reason
      });
    }

    private async Task NotifyCustomerStatusChangeAsync(Repair repair, RepairStatus previousStatus)
    {
      try
      {
        if (repair.Status == RepairStatus.ReadyForPickup)
        {
          Console.WriteLine($"Customer notified: Repair {repair.Id} is ready for pickup! Email: {repair.Person.Email}");
          await _emailNotificationService.SendRepairReadyForPickupAsync(repair);
        }
        else if (repair.Status == RepairStatus.Cancelled)
        {
          Console.WriteLine($"Customer notified: Repair {repair.Id} has been cancelled! Email: {repair.Person.Email}");
          // await _emailNotificationService.SendRepairCancellationAsync(repair);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error notifying customer: {ex.Message}");
      }
    }
  }
}
