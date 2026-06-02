using Store.Core.DTOs.ProductCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface IProductCategoryService : IGenericService<ProductCategoryDto, ProductCategoryCreateDto, ProductCategoryUpdateDto>
    {
        // Get category by name
        Task<ProductCategoryDto?> GetByNameAsync(string categoryName);

        // Get category with all its products
        Task<ProductCategoryDto?> GetWithProductsAsync(int categoryId);

        // Check if category name already exists
        Task<bool> CategoryExistsAsync(string categoryName);
    }
}
