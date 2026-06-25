using Store.Core.DTOs.Review;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================
        // Mapping Methods
        // ==================================================
        private ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                ReviewID = review.ReviewID,
                ProductID = review.ProductID,
                CustomerID = review.CustomerID,
                CustomerName = review.Customer?.Name ?? string.Empty,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                ReviewDate = review.ReviewDate
            };
        }

        private Review MapToEntity(ReviewCreateDto dto)
        {
            return new Review
            {
                ProductID = dto.ProductID,
                CustomerID = dto.CustomerID,
                ReviewText = dto.ReviewText,
                Rating = dto.Rating
                // ReviewDate set by DB default
            };
        }

        // ==================================================
        // IGenericService Implementation
        // ==================================================
        public async Task<IEnumerable<ReviewDto>> GetAllAsync()
        {
            var reviews = await _unitOfWork.Reviews.GetAllAsync();
            return reviews.Select(r => MapToDto(r));
        }

        public async Task<ReviewDto?> GetByIdAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) return null;
            return MapToDto(review);
        }

        public async Task<ReviewDto> CreateAsync(ReviewCreateDto createDto)
        {
            // Business logic — check if customer already reviewed this product
            var alreadyReviewed = await _unitOfWork.Reviews
                                                   .ReviewExistsAsync(createDto.ProductID, createDto.CustomerID);
            if (alreadyReviewed)
                throw new InvalidOperationException("You have already reviewed this product.");


            var customerOrders = await _unitOfWork.Orders
                                               .GetByCustomerAsync(createDto.CustomerID);

            var hasOrderedAndReceived = customerOrders
                .Any(o =>
                    o.Status == 3 &&  // 3 = Delivered
                    o.OrderItem != null &&
                    o.OrderItem.Any(oi => oi.ProductID == createDto.ProductID));

            if (!hasOrderedAndReceived)
                throw new InvalidOperationException(
                    "You can only review products you have purchased and received.");


            var review = MapToEntity(createDto);
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(review);
        }

        public async Task<ReviewDto?> UpdateAsync(int id, ReviewUpdateDto updateDto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) return null;

            // Only text and rating can be updated
            review.ReviewText = updateDto.ReviewText;
            review.Rating = updateDto.Rating;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(review);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) return false;

            _unitOfWork.Reviews.Delete(review);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // IReviewService Specific Methods
        // ==================================================
        public async Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetByProductAsync(productId);
            return reviews.Select(r => MapToDto(r));
        }

        public async Task<IEnumerable<ReviewDto>> GetByCustomerAsync(int customerId)
        {
            var reviews = await _unitOfWork.Reviews.GetByCustomerAsync(customerId);
            return reviews.Select(r => MapToDto(r));
        }

        public async Task<decimal> GetAverageRatingAsync(int productId)
        {
            return await _unitOfWork.Reviews.GetAverageRatingAsync(productId);
        }

        public async Task<bool> ReviewExistsAsync(int productId, int customerId)
        {
            return await _unitOfWork.Reviews.ReviewExistsAsync(productId, customerId);
        }
    }
}
