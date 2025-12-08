using padelya_api.Constants;
using padelya_api.DTOs.Tournament;
using padelya_api.Models.Tournament;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public interface ITournamentService
    {
        Task<Tournament?> CreateTournamentAsync(CreateTournamentDto tournamentDto);
        Task<List<TournamentResponseDto>> GetTournamentsAsync();
        Task<bool> DeleteTournamentAsync(int id);
        Task<Tournament?> UpdateTournamentAsync(int id, UpdateTournamentDto updateTournamentDto);
        Task<TournamentResponseDto?> GetTournamentByIdAsync(int id);
        Task<Tournament?> UpdateTournamentStatusAsync(int id, TournamentStatus newStatus);
        Task<TournamentEnrollment?> EnrollPlayerAsync(int tournamentId, TournamentEnrollmentDto enrollmentDto);
        Task<TournamentEnrollmentInitPointDto> EnrollWithPaymentAsync(int tournamentId, TournamentEnrollmentWithPaymentDto enrollmentDto);
        Task<bool> CancelEnrollmentAsync(int tournamentId, int userId);
        Task<TournamentReportDto> GetTournamentReportAsync(DateTime startDate, DateTime endDate);

    }
}
