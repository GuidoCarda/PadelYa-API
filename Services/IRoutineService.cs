using padelya_api.DTOs.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public interface IRoutineService
    {
        Task<ResponseMessage<RoutineDto>> CreateRoutineAsync(RoutineCreateDto createDto, int teacherId);
        Task<ResponseMessage<RoutineDto>> UpdateRoutineAsync(int routineId, RoutineUpdateDto updateDto, int teacherId);
        Task<ResponseMessage<RoutineDto>> GetRoutineByIdAsync(int routineId);
        Task<ResponseMessage<List<RoutineDto>>> GetRoutinesByTeacherAsync(int teacherId);
        Task<ResponseMessage<List<RoutineDto>>> GetRoutinesByPlayerAsync(int playerId);
        Task<ResponseMessage<bool>> DeleteRoutineAsync(int routineId, int teacherId);
    }
}

