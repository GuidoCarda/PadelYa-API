using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Product;
using padelya_api.Services.Product;
using padelya_api.Services.File;
using padelya_api.Shared;
using Microsoft.EntityFrameworkCore;
using padelya_api.Data;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IFileService _fileService;
        private readonly PadelYaDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService, 
            IFileService fileService,
            PadelYaDbContext context,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _fileService = fileService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los productos del catálogo
        /// </summary>
        /// <param name="includeInactive">Incluir productos inactivos</param>
        /// <returns>Lista de productos</returns>
        [HttpGet]
        [AllowAnonymous] // Permitir ver productos sin autenticación
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetAllProducts(
            [FromQuery] bool includeInactive = false)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync(includeInactive);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos");
                return StatusCode(500, ResponseMessage.Error("Error al obtener la lista de productos"));
            }
        }

        /// <summary>
        /// Obtiene un producto específico por su ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Detalles del producto</returns>
        [HttpGet("{id}")]
        [AllowAnonymous] // Permitir ver detalles sin autenticación
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound(ResponseMessage.Error("Producto no encontrado"));
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {ProductId}", id);
                return StatusCode(500, ResponseMessage.Error("Error al obtener el producto"));
            }
        }

        /// <summary>
        /// Crea un nuevo producto en el catálogo
        /// RF7 - Agregar nuevo producto al catálogo
        /// </summary>
        /// <param name="createDto">Datos del producto a crear</param>
        /// <returns>Producto creado</returns>
        [HttpPost]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("product:create")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _productService.CreateProductAsync(createDto);
                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = product.Id },
                    product
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseMessage.Error(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto");
                return StatusCode(500, ResponseMessage.Error("Error al crear el producto"));
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// RF8 - Modificar producto existente
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="updateDto">Datos a actualizar</param>
        /// <returns>Producto actualizado</returns>
        [HttpPut("{id}")]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("product:edit")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _productService.UpdateProductAsync(id, updateDto);

                if (product == null)
                {
                    return NotFound(ResponseMessage.Error("Producto no encontrado"));
                }

                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseMessage.Error(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto con ID {ProductId}", id);
                return StatusCode(500, ResponseMessage.Error("Error al actualizar el producto"));
            }
        }

        /// <summary>
        /// Elimina un producto del catálogo
        /// RF9 - Eliminar producto del catálogo
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("product:delete")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);

                if (!result)
                {
                    return NotFound(ResponseMessage.Error("Producto no encontrado"));
                }

                return Ok(ResponseMessage.SuccessMessage("Producto eliminado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseMessage.Error(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID {ProductId}", id);
                return StatusCode(500, ResponseMessage.Error("Error al eliminar el producto"));
            }
        }

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        /// <returns>Lista de categorías</returns>
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            try
            {
                var categories = await _productService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de categorías");
                return StatusCode(500, ResponseMessage.Error("Error al obtener las categorías"));
            }
        }

        /// <summary>
        /// Sube una o más imágenes para un producto (máximo 3)
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="files">Archivos de imagen</param>
        /// <returns>Lista de URLs de las imágenes subidas</returns>
        [HttpPost("{productId}/images")]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("product:edit")]
        public async Task<ActionResult> UploadProductImages(int productId, [FromForm] IFormFileCollection files)
        {
            try
            {
                // Verificar que el producto existe
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    return NotFound(ResponseMessage.Error("Producto no encontrado"));
                }

                // Validar cantidad de imágenes actuales + nuevas
                var currentImagesCount = product.Images.Count;
                if (currentImagesCount + files.Count > 3)
                {
                    return BadRequest(ResponseMessage.Error($"El producto ya tiene {currentImagesCount} imagen(es). Solo puedes subir {3 - currentImagesCount} más (máximo 3 imágenes en total)"));
                }

                if (files.Count == 0)
                {
                    return BadRequest(ResponseMessage.Error("No se recibieron archivos"));
                }

                var uploadedImages = new List<ProductImageDto>();

                foreach (var file in files)
                {
                    if (!_fileService.IsValidImage(file))
                    {
                        _logger.LogWarning("Archivo inválido omitido al subir imágenes para producto {ProductId}", productId);
                        continue; // Saltar archivos inválidos
                    }

                    string imageUrl;
                    try
                    {
                        // Guardar imagen
                        imageUrl = await _fileService.SaveProductImageAsync(file);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Loguear y continuar con el siguiente archivo en vez de fallar todo el request
                        _logger.LogWarning(ex, "No se pudo guardar un archivo al subir imágenes para producto {ProductId}", productId);
                        continue;
                    }

                    // Determinar orden y si es primaria
                    var displayOrder = product.Images.Count + 1;
                    var isPrimary = product.Images.Count == 0; // La primera es primaria

                    // Crear registro en BD
                    var productImage = new Models.Ecommerce.ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = imageUrl,
                        DisplayOrder = displayOrder,
                        IsPrimary = isPrimary,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.ProductImages.Add(productImage);
                    await _context.SaveChangesAsync();

                    uploadedImages.Add(new ProductImageDto
                    {
                        Id = productImage.Id,
                        ImageUrl = productImage.ImageUrl,
                        DisplayOrder = productImage.DisplayOrder,
                        IsPrimary = productImage.IsPrimary
                    });
                }

                if (uploadedImages.Count == 0)
                {
                    return BadRequest(ResponseMessage.Error("No se pudo subir ninguna imagen válida"));
                }

                _logger.LogInformation("Se subieron {Count} imágenes para el producto {ProductId}", uploadedImages.Count, productId);

                return Ok(ResponseMessage<List<ProductImageDto>>.SuccessResult(
                    uploadedImages, 
                    $"Se subieron {uploadedImages.Count} imagen(es) exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imágenes para el producto {ProductId}", productId);
                return StatusCode(500, ResponseMessage.Error("Error al subir las imágenes"));
            }
        }

        /// <summary>
        /// Elimina una imagen de un producto
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="imageId">ID de la imagen</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{productId}/images/{imageId}")]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("product:edit")]
        public async Task<ActionResult> DeleteProductImage(int productId, int imageId)
        {
            try
            {
                var image = await _context.ProductImages
                    .FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == productId);

                if (image == null)
                {
                    return NotFound(ResponseMessage.Error("Imagen no encontrada"));
                }

                // Eliminar archivo físico
                await _fileService.DeleteProductImageAsync(image.ImageUrl);

                // Eliminar registro de BD
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();

                // Reorganizar orden de imágenes restantes
                var remainingImages = await _context.ProductImages
                    .Where(i => i.ProductId == productId)
                    .OrderBy(i => i.DisplayOrder)
                    .ToListAsync();

                for (int i = 0; i < remainingImages.Count; i++)
                {
                    remainingImages[i].DisplayOrder = i + 1;
                    // Si eliminamos la primaria, hacer primaria a la primera restante
                    if (i == 0)
                    {
                        remainingImages[i].IsPrimary = true;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Imagen {ImageId} eliminada del producto {ProductId}", imageId, productId);

                return Ok(ResponseMessage.SuccessMessage("Imagen eliminada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la imagen {ImageId} del producto {ProductId}", imageId, productId);
                return StatusCode(500, ResponseMessage.Error("Error al eliminar la imagen"));
            }
        }

        /// <summary>
        /// Establece una imagen como primaria
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="imageId">ID de la imagen</param>
        /// <returns>Confirmación</returns>
        [HttpPut("{productId}/images/{imageId}/set-primary")]
        // TODO: Agregar autorización con permisos de ecommerce
        // [RequirePermission("product:edit")]
        public async Task<ActionResult> SetPrimaryImage(int productId, int imageId)
        {
            try
            {
                var image = await _context.ProductImages
                    .FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == productId);

                if (image == null)
                {
                    return NotFound(ResponseMessage.Error("Imagen no encontrada"));
                }

                // Quitar la marca de primaria de todas las imágenes del producto
                var allImages = await _context.ProductImages
                    .Where(i => i.ProductId == productId)
                    .ToListAsync();

                foreach (var img in allImages)
                {
                    img.IsPrimary = img.Id == imageId;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Imagen {ImageId} establecida como primaria para el producto {ProductId}", imageId, productId);

                return Ok(ResponseMessage.SuccessMessage("Imagen primaria actualizada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer imagen primaria");
                return StatusCode(500, ResponseMessage.Error("Error al actualizar la imagen primaria"));
            }
        }

        /// <summary>
        /// Permite subir una imagen individual (útil al crear un producto antes de tener productId)
        /// Devuelve imageUrl e imagePath
        /// </summary>
        [HttpPost("upload-image")]
        public async Task<ActionResult> UploadImage([FromForm] IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest(ResponseMessage.Error("No se recibió ningún archivo"));
                }

                if (!_fileService.IsValidImage(file))
                {
                    return BadRequest(ResponseMessage.Error("Archivo de imagen inválido"));
                }

                string imageUrl;
                try
                {
                    imageUrl = await _fileService.SaveProductImageAsync(file);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "No se pudo guardar la imagen enviada a upload-image");
                    return BadRequest(ResponseMessage.Error("No se pudo guardar la imagen"));
                }

                var imagePath = _fileService.GetImagePath(imageUrl);

                // Devolver estructura simple que el frontend espera
                return Ok(new { imageUrl = imageUrl, imagePath = imagePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imagen a upload-image");
                return StatusCode(500, ResponseMessage.Error("Error al subir la imagen"));
            }
        }
    }
}

