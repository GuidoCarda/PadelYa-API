
using padelya_api.DTOs.Repair;
using padelya_api.Models.Repair;

namespace padelya_api.Services
{
  public interface IRepairService
  {
    Task<IEnumerable<RepairResponseDto>> GetAllAsync(string? email = null, string? status = null, string? startDate = null, string? endDate = null);
    Task<IEnumerable<RepairResponseDto>> GetMyRepairsAsync();
    Task<Repair?> GetByIdAsync(int id);
    Task<Repair> CreateAsync(CreateRepairDto dto);
    Task<Repair> UpdateAsync(int id, UpdateRepairDto dto);
    Task<Repair> UpdateStatusAsync(int id, UpdateStatusDto dto);
    Task<Repair> CancelAsync(int id, CancelRepairDto cancellationDto);
    Task<Repair> RegisterPaymentAsync(int id, RegisterRepairPaymentDto dto);
    Task<RepairReportDto> GetRepairReportAsync(DateTime startDate, DateTime endDate);
  }
}