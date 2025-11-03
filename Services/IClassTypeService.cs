using padelya_api.DTOs.Lesson;
using padelya_api.Shared;

namespace padelya_api.Services
{
    public interface IClassTypeService
    {
        Task<ResponseMessage<List<ClassTypeDto>>> GetAllAsync();
        Task<ResponseMessage<ClassTypeDto>> GetByIdAsync(int id);
        Task<ResponseMessage<ClassTypeDto>> CreateAsync(ClassTypeCreateDto createDto);
        Task<ResponseMessage<ClassTypeDto>> UpdateAsync(ClassTypeUpdateDto updateDto);
        Task<ResponseMessage<bool>> DeleteAsync(int id);
    }
}

