using Microsoft.AspNetCore.Mvc;
using Store.API.Helpers;
using Store.Core.DTOs.ProductCategory;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _categoryService;

        public ProductCategoryController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // ==================================================
        // GET api/productcategory
        // ==================================================
        [HttpGet]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> GetById(int id)
        {
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> GetWithProducts(int id)
        {
            var category = await _categoryService.GetWithProductsAsync(id);
            if (category == null)
                return NotFound(ApiResponse<ProductCategoryDto>.Fail($"Category with ID {id} was not found"));

            return Ok(ApiResponse<ProductCategoryDto>.Ok(category, "Category with products retrieved successfully"));
        }

        // ==================================================
        // POST api/productcategory
        // ==================================================
        [HttpPost]
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
