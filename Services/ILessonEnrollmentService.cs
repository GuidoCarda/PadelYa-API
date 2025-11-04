using padelya_api.DTOs.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public interface ILessonEnrollmentService
    {
        Task<ResponseMessage<LessonEnrollmentResponseDto>> EnrollStudentAsync(LessonEnrollmentCreateDto createDto, int? userId = null);
        Task<ResponseMessage<bool>> CancelEnrollmentAsync(int enrollmentId, int? userId = null);
        Task<ResponseMessage<List<LessonEnrollmentListDto>>> GetEnrollmentsByLessonAsync(int lessonId);
        Task<ResponseMessage<List<LessonEnrollmentResponseDto>>> GetEnrollmentsByStudentAsync(int personId);
        Task<ResponseMessage<bool>> AdminEnrollStudentAsync(int lessonId, int personId);
        Task<ResponseMessage<bool>> AdminRemoveEnrollmentAsync(int enrollmentId);
        Task<ResponseMessage<bool>> UpdateEnrollmentPaymentStatusAsync(int enrollmentId, string paymentStatus);
    }
}

