using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Store.API.Helpers;
using Store.API.Services;
using Store.Core.DTOs.Product;
using Store.Core.Helpers;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IImageUploadService _imageUploadService;

        public ProductController(IProductService productService,IImageUploadService imageUploadService)
        {
            _productService = productService;
            _imageUploadService = imageUploadService;
        }

        // ==================================================
        // GET api/product?pageNumber=1&pageSize=10
        // Get all products with pagination
        // ==================================================
        [HttpGet]
        [AllowAnonymous]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<ProductSummaryDto>>>> GetAll(
            [FromQuery] PaginationParams pagination)
        {
            var products = await _productService.GetAllAsync();

            // Apply pagination
            var totalCount = products.Count();
            var pagedProducts = products
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            var result = new PaginatedResult<ProductSummaryDto>(
                pagedProducts,
                totalCount,
                pagination.PageNumber,
                pagination.PageSize
            );

            return Ok(ApiResponse<PaginatedResult<ProductSummaryDto>>.Ok(
                result,
                $"Page {pagination.PageNumber} of {result.TotalPages}"
            ));
        }

        // ==================================================
        // GET api/product/1
        // ==================================================
        [HttpGet("{id}")]
        [AllowAnonymous]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductSummaryDto>>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductSummaryDto>.Fail($"Product with ID {id} was not found"));

            return Ok(ApiResponse<ProductSummaryDto>.Ok(product, "Product retrieved successfully"));
        }

        // ==================================================
        // GET api/product/1/details
        // ==================================================
        [HttpGet("{id}/details")]
        [AllowAnonymous]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetDetail(int id)
        {
            var product = await _productService.GetDetailByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductDetailDto>.Fail($"Product with ID {id} was not found"));

            return Ok(ApiResponse<ProductDetailDto>.Ok(product, "Product details retrieved successfully"));
        }

        // ==================================================
        // GET api/product/category/1?pageNumber=1&pageSize=10
        // ==================================================
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<ProductSummaryDto>>>> GetByCategory(
            int categoryId,
            [FromQuery] PaginationParams pagination)
        {
            var products = await _productService.GetByCategoryAsync(categoryId);
            var totalCount = products.Count();
            var paged = products
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            var result = new PaginatedResult<ProductSummaryDto>(
                paged, totalCount, pagination.PageNumber, pagination.PageSize
            );

            return Ok(ApiResponse<PaginatedResult<ProductSummaryDto>>.Ok(
                result, "Products retrieved successfully"
            ));
        }

        // ==================================================
        // GET api/product/search?name=headphones&pageNumber=1&pageSize=10
        // ==================================================
        [HttpGet("search")]
        [AllowAnonymous]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<ProductSummaryDto>>>> Search(
            [FromQuery] string name,
            [FromQuery] PaginationParams pagination)
        {
            var products = await _productService.SearchByNameAsync(name);
            var totalCount = products.Count();
            var paged = products
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            var result = new PaginatedResult<ProductSummaryDto>(
                paged, totalCount, pagination.PageNumber, pagination.PageSize
            );

            return Ok(ApiResponse<PaginatedResult<ProductSummaryDto>>.Ok(
                result, "Search results retrieved successfully"
            ));
        }

        // ==================================================
        // GET api/product/price?min=10&max=100
        // ==================================================
        [HttpGet("price")]
        [AllowAnonymous]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductSummaryDto>>>> GetByPriceRange(
            [FromQuery] decimal min,
            [FromQuery] decimal max)
        {
            var products = await _productService.GetByPriceRangeAsync(min, max);
            return Ok(ApiResponse<IEnumerable<ProductSummaryDto>>.Ok(
                products, "Products retrieved successfully"
            ));
        }

        // ==================================================
        // POST api/product
        // ==================================================
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ProductSummaryDto>>> Create(ProductCreateDto createDto)
        {
            var product = await _productService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = product.ProductID },
                                   ApiResponse<ProductSummaryDto>.Ok(product, "Product created successfully"));
        }

        // ==================================================
        // POST api/product/1/images
        // ==================================================
        [HttpPost("{id}/images")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

       public async Task<ActionResult<ApiResponse<ProductImageDto>>> AddImage(int id,IFormFile file,  [FromForm] int imageOrder = 0)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductImageDto>.NotFound(
                    $"Product with ID {id} was not found"));

            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<ProductImageDto>.Fail(
                    "No image file provided",
                    new List<string> { "Please select an image file" }));

            if (!_imageUploadService.IsValidImage(file))
                return BadRequest(ApiResponse<ProductImageDto>.Fail(
                    "Invalid image file",
                    new List<string> { _imageUploadService.GetValidationError(file) }));

            var imageUrl = await _imageUploadService.UploadImageAsync(file, "products");

            var imageDto = new ProductImageCreateDto
            {
                ImageURL = imageUrl,
                ImageOrder = (short)imageOrder
            };

            var savedImage = await _productService.AddImageAsync(id, imageDto);
            return CreatedAtAction(nameof(GetDetail),
                new { id },
                ApiResponse<ProductImageDto>.Ok(savedImage, "Image uploaded successfully", 201));
        }

        // ==================================================
        // PUT api/product/images/reordaring
        // ==================================================

        [HttpPut("{id}/images/reorder")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> ReorderImages(
            int id,
            List<ImageReorderDto> reorderDto)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<bool>.NotFound(
                    $"Product with ID {id} was not found"));

            if (reorderDto == null || !reorderDto.Any())
                return BadRequest(ApiResponse<bool>.Fail(
                    "No images provided",
                    new List<string> { "Please provide at least one image to reorder" }));

            try
            {
                await _productService.ReorderImagesAsync(id, reorderDto);
                return Ok(ApiResponse<bool>.Ok(true, "Images reordered successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>.Fail(
                    "Reorder failed",
                    new List<string> { ex.Message }));
            }
        }

        // ==================================================
        // PUT api/product/1
        // ==================================================
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductSummaryDto>>> Update(
            int id,
            ProductUpdateDto updateDto)
        {
            var product = await _productService.UpdateAsync(id, updateDto);
            if (product == null)
                return NotFound(ApiResponse<ProductSummaryDto>.Fail($"Product with ID {id} was not found"));

            return Ok(ApiResponse<ProductSummaryDto>.Ok(product, "Product updated successfully"));
        }

        // ==================================================
        // DELETE api/product/1
        // ==================================================
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Product with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Product deleted successfully"));
        }

        // ==================================================
        // DELETE api/product/images/1
        // ==================================================
        [HttpDelete("images/{imageId}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

       
        public async Task<ActionResult<ApiResponse<bool>>> RemoveImage(int imageId)
        {
            // Get URL before deleting from DB
            var imageUrl = await _productService.GetImageUrlAsync(imageId);

            var result = await _productService.RemoveImageAsync(imageId);
            if (!result)
                return NotFound(ApiResponse<bool>.NotFound(
                    $"Image with ID {imageId} was not found"));

            // Also delete file from server
            if (!string.IsNullOrEmpty(imageUrl))
                await _imageUploadService.DeleteImageAsync(imageUrl);

            return Ok(ApiResponse<bool>.Ok(true, "Image deleted successfully"));
        }
    }
}
