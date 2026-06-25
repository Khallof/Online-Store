using Store.Core.DTOs.ProductCategory;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================
        // Mapping Methods
        // ==================================================
        private ProductCategoryDto MapToDto(ProductCategory category)
        {
            return new ProductCategoryDto
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName
            };
        }

        private ProductCategory MapToEntity(ProductCategoryCreateDto dto)
        {
            return new ProductCategory
            {
                CategoryName = dto.CategoryName
            };
        }

        // ==================================================
        // IGenericService Implementation
        // ==================================================
        public async Task<IEnumerable<ProductCategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.ProductCategories.GetAllAsync();
            return categories.Select(c => MapToDto(c));
        }

        public async Task<ProductCategoryDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.ProductCategories.GetByIdAsync(id);
            if (category == null) return null;
            return MapToDto(category);
        }

        public async Task<ProductCategoryDto> CreateAsync(ProductCategoryCreateDto createDto)
        {
            var exists = await _unitOfWork.ProductCategories
                                 .CategoryExistsAsync(createDto.CategoryName);
            if (exists)
                throw new InvalidOperationException(
                    $"Category '{createDto.CategoryName}' already exists.");

            var category = MapToEntity(createDto);
            await _unitOfWork.ProductCategories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<ProductCategoryDto?> UpdateAsync(int id, ProductCategoryUpdateDto updateDto)
        {
            var category = await _unitOfWork.ProductCategories.GetByIdAsync(id);
            if (category == null) return null;

            if (category.CategoryName != updateDto.CategoryName)
            {
                var exists = await _unitOfWork.ProductCategories
                                              .CategoryExistsAsync(updateDto.CategoryName);
                if (exists)
                    throw new InvalidOperationException(
                        $"Category '{updateDto.CategoryName}' already exists.");
            }

            category.CategoryName = updateDto.CategoryName;

            _unitOfWork.ProductCategories.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.ProductCategories
                                            .GetWithProductsAsync(id);
            if (category == null) return false;

           
            if (category.ProductCatalog != null && category.ProductCatalog.Any())
                throw new InvalidOperationException(
                    $"Cannot delete category '{category.CategoryName}' " +

                    $"because it has {category.ProductCatalog.Count()} products. " +

                    $"Please move or delete the products first.");

            _unitOfWork.ProductCategories.Delete(
            await _unitOfWork.ProductCategories.GetByIdAsync(id) ?? new ProductCategory());
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // IProductCategoryService Specific Methods
        // ==================================================
        public async Task<ProductCategoryDto?> GetByNameAsync(string categoryName)
        {
            var category = await _unitOfWork.ProductCategories.GetByNameAsync(categoryName);
            if (category == null) return null;
            return MapToDto(category);
        }

        public async Task<ProductCategoryDto?> GetWithProductsAsync(int categoryId)
        {
            var category = await _unitOfWork.ProductCategories.GetWithProductsAsync(categoryId);
            if (category == null) return null;
            return MapToDto(category);
        }

        public async Task<bool> CategoryExistsAsync(string categoryName)
        {
            return await _unitOfWork.ProductCategories.CategoryExistsAsync(categoryName);
        }
    }
}
