using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
    {
        // Get category with all its products
        Task<ProductCategory?> GetWithProductsAsync(int categoryId);

        // Get category by name (for duplicate check)
        Task<ProductCategory?> GetByNameAsync(string categoryName);

        // Check if category name already exists
        Task<bool> CategoryExistsAsync(string categoryName);
    }
}
