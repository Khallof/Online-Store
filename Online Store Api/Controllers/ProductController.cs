using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Store.API.Helpers;
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

        public ProductController(IProductService productService)
        {
            _productService = productService;
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
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductImageDto>>> AddImage(
            int id,
            ProductImageCreateDto imageDto)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductImageDto>.Fail($"Product with ID {id} was not found"));

            var image = await _productService.AddImageAsync(id, imageDto);
            return CreatedAtAction(nameof(GetDetail),
                                   new { id },
                                   ApiResponse<ProductImageDto>.Ok(image, "Image added successfully"));
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
            var result = await _productService.RemoveImageAsync(imageId);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Image with ID {imageId} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Image removed successfully"));
        }
    }
}
