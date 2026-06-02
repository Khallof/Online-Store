using Store.Core.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface IReviewService : IGenericService<ReviewDto, ReviewCreateDto, ReviewUpdateDto>
    {
        // Get all reviews for a specific product
        Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId);

        // Get all reviews by a specific customer
        Task<IEnumerable<ReviewDto>> GetByCustomerAsync(int customerId);

        // Get average rating for a product
        Task<decimal> GetAverageRatingAsync(int productId);

        // Check if customer already reviewed this product
        Task<bool> ReviewExistsAsync(int productId, int customerId);
    }
}
