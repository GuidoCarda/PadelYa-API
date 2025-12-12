using Microsoft.AspNetCore.Http;

namespace padelya_api.Services.File
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        
        // Ruta ÚNICA para almacenar imágenes de productos
        // Ubicación lógica: /images/products/
        // Ubicación física: wwwroot/images/products/
        private const string ProductImagesFolder = "images/products";

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // Validar tamaño
            if (file.Length > MaxFileSize)
            {
                return false;
            }

            // Validar extensión
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return false;
            }

            // Validar tipo MIME
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return false;
            }

            return true;
        }

        public async Task<string> SaveProductImageAsync(IFormFile file)
        {
            try
            {
                if (!IsValidImage(file))
                {
                    throw new InvalidOperationException("Archivo de imagen inválido");
                }

                // Determinar ruta de upload con fallback si WebRootPath no está configurado
                var webRoot = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    // Preferir ContentRootPath (ruta del proyecto) para localizar wwwroot cuando WebRootPath no está seteado
                    _logger.LogWarning("WebRootPath no configurado, usando ContentRootPath/wwwroot como fallback");
                    webRoot = Path.Combine(_environment.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot");
                }

                _logger.LogInformation("FileService: WebRootPath='{WebRoot}', ContentRootPath='{ContentRoot}'", _environment.WebRootPath, _environment.ContentRootPath);

                // Crear directorio si no existe
                var uploadPath = Path.Combine(webRoot, ProductImagesFolder);
                _logger.LogInformation("FileService: uploadPath='{UploadPath}'", uploadPath);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generar nombre único para el archivo
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Retornar URL relativa esperada por el frontend
                var imageUrl = $"/{ProductImagesFolder}/{fileName}".Replace("\\", "/");
                _logger.LogInformation("Imagen guardada exitosamente: {ImageUrl} (físico: {FilePath})", imageUrl, filePath);

                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la imagen");
                // Lanzar una InvalidOperationException para que el controller lo maneje como BadRequest si aplica
                throw new InvalidOperationException("No se pudo guardar la imagen");
            }
        }

        public async Task<bool> DeleteProductImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    return false;
                }

                // Extraer ruta del archivo
                var fileName = Path.GetFileName(imageUrl);
                var webRootForDelete = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootForDelete))
                {
                    webRootForDelete = Path.Combine(_environment.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot");
                }
                var filePath = Path.Combine(webRootForDelete, ProductImagesFolder, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    await Task.Run(() => System.IO.File.Delete(filePath));
                    _logger.LogInformation("Imagen eliminada exitosamente: {ImageUrl}", imageUrl);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la imagen: {ImageUrl}", imageUrl);
                return false;
            }
        }

        public string GetImagePath(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return string.Empty;
            }

            var fileName = Path.GetFileName(imageUrl);
            var webRootForGet = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootForGet))
            {
                webRootForGet = Path.Combine(_environment.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot");
            }
            return Path.Combine(webRootForGet, ProductImagesFolder, fileName);
        }
    }
}

