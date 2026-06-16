using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Store.API.Helpers;
using Store.Core.DTOs.Payment;
using Store.Core.DTOs.ProductCategory;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _categoryService;

        public ProductCategoryController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private bool IsAdminOrSameCustomer(int customerId)
        {
            var loggedInId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var loggedInRole = User.FindFirst("role")?.Value;
            return loggedInRole == "Admin" || loggedInId == customerId;
        }

        // ==================================================
        // GET api/productcategory
        // ==================================================
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductCategoryDto>>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ProductCategoryDto>>.Ok(categories, "Categories retrieved successfully"));
        }

        // ==================================================
        // GET api/productcategory/1
        // ==================================================
        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> GetById(int id)
        {

            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<ProductCategoryDto>.Fail("You can only view your own data"));

            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<ProductCategoryDto>.Fail($"Category with ID {id} was not found"));

            return Ok(ApiResponse<ProductCategoryDto>.Ok(category, "Category retrieved successfully"));
        }

        // ==================================================
        // GET api/productcategory/1/products
        // Get category with all its products
        // ==================================================
        [HttpGet("{id}/products")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> GetWithProducts(int id)
        {
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<ProductCategoryDto>.Fail("You can only view your own data"));
            var category = await _categoryService.GetWithProductsAsync(id);
            if (category == null)
                return NotFound(ApiResponse<ProductCategoryDto>.Fail($"Category with ID {id} was not found"));

            return Ok(ApiResponse<ProductCategoryDto>.Ok(category, "Category with products retrieved successfully"));
        }

        // ==================================================
        // POST api/productcategory
        // ==================================================
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> Create(ProductCategoryCreateDto createDto)
        {
            if (await _categoryService.CategoryExistsAsync(createDto.CategoryName))
                return BadRequest(ApiResponse<ProductCategoryDto>.Fail("Category name already exists"));

            var category = await _categoryService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = category.CategoryID },
                                   ApiResponse<ProductCategoryDto>.Ok(category, "Category created successfully"));
        }

        // ==================================================
        // PUT api/productcategory/1
        // ==================================================
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> Update(int id, ProductCategoryUpdateDto updateDto)
        {
            var category = await _categoryService.UpdateAsync(id, updateDto);
            if (category == null)
                return NotFound(ApiResponse<ProductCategoryDto>.Fail($"Category with ID {id} was not found"));

            return Ok(ApiResponse<ProductCategoryDto>.Ok(category, "Category updated successfully"));
        }

        // ==================================================
        // DELETE api/productcategory/1
        // ==================================================
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Category with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Category deleted successfully"));
        }
    }
}
