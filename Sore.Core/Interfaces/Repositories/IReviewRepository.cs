using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        // Get all reviews for a specific product
        Task<IEnumerable<Review>> GetByProductAsync(int productId);

        // Get all reviews by a specific customer
        Task<IEnumerable<Review>> GetByCustomerAsync(int customerId);

        // Get a specific review by product and customer
        Task<Review?> GetByProductAndCustomerAsync(int productId, int customerId);

        // Get average rating for a product
        Task<decimal> GetAverageRatingAsync(int productId);

        // Check if customer already reviewed this product
        Task<bool> ReviewExistsAsync(int productId, int customerId);
    }
}
