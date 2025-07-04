

using padelya_api.DTOs;
using padelya_api.DTOs.Court;
using padelya_api.Models;

namespace padelya_api.Services
{
    public interface IComplexService
    {
        Task<Complex> GetComplexAsync();
        Task<List<Court>> GetCourtsAsync();
        Task<Court?> GetCourtByIdAsync(int id);
        Task<Court?> CreateCourtAsync(CreateCourtDto request);
        Task<Court?> UpdateCourtAsync(int id, UpdateCourtDto request);
        Task<bool> DeleteCourtAsync(int id);
    }
}
