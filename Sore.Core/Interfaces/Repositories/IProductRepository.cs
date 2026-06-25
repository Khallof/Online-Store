using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<ProductCatalog>
    {
        // Get all products with their category and images
        Task<IEnumerable<ProductCatalog>> GetAllWithDetailsAsync();

        // Get a single product with category and all images
        Task<ProductCatalog?> GetWithDetailsAsync(int productId);

        // Get all products by category
        Task<IEnumerable<ProductCatalog>> GetByCategoryAsync(int categoryId);

        // Get products by name search
        Task<IEnumerable<ProductCatalog>> SearchByNameAsync(string name);

        // Get products filtered by price range
        Task<IEnumerable<ProductCatalog>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // Check if product code already exists
        Task<bool> ProductExistsAsync(string productName);

        // Update stock quantity after an order
        Task UpdateStockAsync(int productId, int quantity);

        Task<ProductImages> AddImageAsync(ProductImages image);

        Task<bool> RemoveImageAsync(int imageId);

        Task<ProductImages?> GetImageByIdAsync(int imageId);

        // Update image order
        Task UpdateImageOrderAsync(int imageId, short imageOrder);

    }
}
