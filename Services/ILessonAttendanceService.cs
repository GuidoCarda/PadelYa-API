using padelya_api.DTOs.Lesson;
using padelya_api.Models.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public interface ILessonAttendanceService
    {
        Task<ResponseMessage<LessonAttendanceDto>> RecordAttendanceAsync(LessonAttendanceCreateDto createDto, int teacherId);
        Task<ResponseMessage<List<LessonAttendanceDto>>> RecordBulkAttendanceAsync(LessonAttendanceBulkDto bulkDto, int teacherId);
        Task<ResponseMessage<List<LessonAttendanceDto>>> GetAttendanceByLessonAsync(int lessonId);
        Task<ResponseMessage<List<LessonAttendanceDto>>> GetAttendanceByStudentAsync(int personId);
        Task<ResponseMessage<LessonAttendanceDto>> UpdateAttendanceAsync(int attendanceId, AttendanceStatus status, string? notes, int teacherId);
        Task<ResponseMessage<AttendanceStatisticsDto>> GetAttendanceStatisticsByStudentAsync(int personId);
    }
}

