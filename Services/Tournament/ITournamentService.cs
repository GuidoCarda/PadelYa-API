using padelya_api.DTOs.Tournament;
using padelya_api.Models.Tournament;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public interface ITournamentService
    {
        Task<Tournament?> CreateTournamentAsync(CreateTournamentDto tournamentDto);
        Task<List<Tournament>> GetTournamentsAsync();
    }
}