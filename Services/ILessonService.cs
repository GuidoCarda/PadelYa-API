using padelya_api.DTOs.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public interface ILessonService
    {
        Task<ResponseMessage<LessonResponseDto>> CreateLessonAsync(LessonCreateDto createDto);
        Task<ResponseMessage<List<LessonResponseDto>>> CreateRecurrentLessonsAsync(LessonCreateDto createDto);
        Task<ResponseMessage<LessonResponseDto>> GetLessonByIdAsync(int id);
        Task<ResponseMessage<List<LessonListDto>>> GetLessonsAsync(LessonFilterDto filterDto);
        Task<ResponseMessage<LessonResponseDto>> UpdateLessonAsync(LessonUpdateDto updateDto);
        Task<ResponseMessage<bool>> DeleteLessonAsync(int id);
        Task<ResponseMessage<bool>> CancelLessonAsync(int id);
        Task<ResponseMessage<List<LessonListDto>>> GetLessonsByTeacherAsync(int teacherId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ResponseMessage<List<LessonListDto>>> GetLessonsByCourtAsync(int courtId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ResponseMessage<bool>> ValidateCourtAvailabilityAsync(int courtId, DateTime date, TimeOnly startTime, TimeOnly endTime, int? excludeLessonId = null);
    }
} 