using padelya_api.DTOs.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public interface IExerciseService
    {
        Task<ResponseMessage<List<ExerciseDto>>> GetAllExercisesAsync();
        Task<ResponseMessage<ExerciseDto>> GetExerciseByIdAsync(int exerciseId);
        Task<ResponseMessage<ExerciseDto>> CreateExerciseAsync(ExerciseCreateDto createDto);
        Task<ResponseMessage<ExerciseDto>> UpdateExerciseAsync(int exerciseId, ExerciseUpdateDto updateDto);
        Task<ResponseMessage<bool>> DeleteExerciseAsync(int exerciseId);
    }
}

