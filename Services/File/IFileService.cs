using Microsoft.AspNetCore.Http;

namespace padelya_api.Services.File
{
    public interface IFileService
    {
        Task<string> SaveProductImageAsync(IFormFile file);
        Task<bool> DeleteProductImageAsync(string imageUrl);
        bool IsValidImage(IFormFile file);
        string GetImagePath(string imageUrl);
    }
}

