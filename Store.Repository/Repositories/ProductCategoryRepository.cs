using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;

namespace Store.Repository.Repositories
{
    public class ProductCategoryRepository : GenericRepository<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ProductCategory?> GetByNameAsync(string categoryName)
        {
            return await _context.ProductCategories
                                 .FirstOrDefaultAsync(c => c.CategoryName == categoryName);
        }

        public async Task<ProductCategory?> GetWithProductsAsync(int categoryId)
        {
            return await _context.ProductCategories
                                 .Include(c => c.ProductCatalog)
                                 .FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        }

        public async Task<bool> CategoryExistsAsync(string categoryName)
        {
            return await _context.ProductCategories
                                 .AnyAsync(c => c.CategoryName == categoryName);
        }
    }
}
