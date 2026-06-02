using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;

namespace Store.Repository.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetByProductAsync(int productId)
        {
            return await _context.Reviews
                                 .Include(r => r.Customer)
                                 .Where(r => r.ProductID == productId)
                                 .OrderByDescending(r => r.ReviewDate)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByCustomerAsync(int customerId)
        {
            return await _context.Reviews
                                 .Include(r => r.ProductCatalog)
                                 .Where(r => r.CustomerID == customerId)
                                 .OrderByDescending(r => r.ReviewDate)
                                 .ToListAsync();
        }

        public async Task<Review?> GetByProductAndCustomerAsync(int productId, int customerId)
        {
            return await _context.Reviews
                                 .FirstOrDefaultAsync(r => r.ProductID == productId &&
                                                           r.CustomerID == customerId);
        }

        public async Task<decimal> GetAverageRatingAsync(int productId)
        {
            var reviews = await _context.Reviews
                                        .Where(r => r.ProductID == productId)
                                        .ToListAsync();

            if (!reviews.Any()) return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<bool> ReviewExistsAsync(int productId, int customerId)
        {
            return await _context.Reviews
                                 .AnyAsync(r => r.ProductID == productId &&
                                                r.CustomerID == customerId);
        }
    }
}
