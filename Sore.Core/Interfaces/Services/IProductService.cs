using Store.Core.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface IProductService : IGenericService<ProductSummaryDto, ProductCreateDto, ProductUpdateDto>
    {
        // Get full product detail with all images
        Task<ProductDetailDto?> GetDetailByIdAsync(int productId);

        // Get all products in a category
        Task<IEnumerable<ProductSummaryDto>> GetByCategoryAsync(int categoryId);

        // Search products by name
        Task<IEnumerable<ProductSummaryDto>> SearchByNameAsync(string name);

        // Get products in a price range
        Task<IEnumerable<ProductSummaryDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // Add an image to a product
        Task<ProductImageDto> AddImageAsync(int productId, ProductImageCreateDto imageDto);

        // Remove an image from a product
        Task<bool> RemoveImageAsync(int imageId);

        // Update stock after an order is placed
        Task UpdateStockAsync(int productId, int quantity);
    }
}
