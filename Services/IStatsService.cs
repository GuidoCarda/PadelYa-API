using padelya_api.DTOs.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public interface IStatsService
    {
        Task<ResponseMessage<StatsDto>> CreateStatsAsync(StatsCreateDto createDto, int teacherId);
        Task<ResponseMessage<StatsDto>> UpdateStatsAsync(int statsId, StatsUpdateDto updateDto, int teacherId);
        Task<ResponseMessage<StatsDto>> GetStatsByIdAsync(int statsId);
        Task<ResponseMessage<List<StatsDto>>> GetStatsByPlayerAsync(int playerId);
        Task<ResponseMessage<List<StatsDto>>> GetStatsByLessonAsync(int lessonId);
        Task<ResponseMessage<bool>> DeleteStatsAsync(int statsId, int teacherId);
    }
}

