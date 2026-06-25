using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;

namespace Store.Repository.Repositories
{
    public class ProductRepository : GenericRepository<ProductCatalog>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductCatalog>> GetAllWithDetailsAsync()
        {
            return await _context.ProductCatalog
                                 .Include(p => p.ProductCategory)
                                 .Include(p => p.ProductImages)
                                 .ToListAsync();
        }

        public async Task<ProductCatalog?> GetWithDetailsAsync(int productId)
        {
            return await _context.ProductCatalog
                                 .Include(p => p.ProductCategory)
                                 .Include(p => p.ProductImages.OrderBy(i => i.ImageOrder))
                                 .FirstOrDefaultAsync(p => p.ProductID == productId);
        }

        public async Task<IEnumerable<ProductCatalog>> GetByCategoryAsync(int categoryId)
        {
            return await _context.ProductCatalog
                                 .Include(p => p.ProductCategory)
                                 .Include(p => p.ProductImages)
                                 .Where(p => p.CategoryID == categoryId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ProductCatalog>> SearchByNameAsync(string name)
        {
            return await _context.ProductCatalog
                                 .Include(p => p.ProductCategory)
                                 .Include(p => p.ProductImages)
                                 .Where(p => p.ProductName.Contains(name))
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ProductCatalog>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.ProductCatalog
                                 .Include(p => p.ProductCategory)
                                 .Include(p => p.ProductImages)
                                 .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                                 .ToListAsync();
        }

        public async Task<bool> ProductExistsAsync(string productName)
        {
            return await _context.ProductCatalog
                                 .AnyAsync(p => p.ProductName == productName);
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _context.ProductCatalog
                                        .FindAsync(productId);
            if (product != null)
            {
                product.QuantityInStock -= quantity;
                _context.ProductCatalog.Update(product);
            }
        }

        public async Task<ProductImages> AddImageAsync(ProductImages image)
        {
            await _context.ProductImages.AddAsync(image);
            return image;
        }

        public async Task<bool> RemoveImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) return false;

            _context.ProductImages.Remove(image);
            return true;
        }


        public async Task<ProductImages?> GetImageByIdAsync(int imageId)
        {
            return await _context.ProductImages
                                 .FindAsync(imageId);
        }

        public async Task UpdateImageOrderAsync(int imageId, short imageOrder)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                image.ImageOrder = imageOrder;
                _context.ProductImages.Update(image);
            }
        }

    }
}
