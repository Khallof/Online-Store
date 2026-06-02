using Microsoft.AspNetCore.Mvc;
using Store.API.Helpers;
using Store.Core.DTOs.Review;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // ==================================================
        // GET api/review
        // ==================================================
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReviewDto>>>> GetAll()
        {
            var reviews = await _reviewService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(reviews, "Reviews retrieved successfully"));
        }

        // ==================================================
        // GET api/review/1
        // ==================================================
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> GetById(int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null)
                return NotFound(ApiResponse<ReviewDto>.Fail($"Review with ID {id} was not found"));

            return Ok(ApiResponse<ReviewDto>.Ok(review, "Review retrieved successfully"));
        }

        // ==================================================
        // GET api/review/product/1
        // Get all reviews for a product
        // ==================================================
        [HttpGet("product/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReviewDto>>>> GetByProduct(int productId)
        {
            var reviews = await _reviewService.GetByProductAsync(productId);
            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(reviews, "Product reviews retrieved successfully"));
        }

        // ==================================================
        // GET api/review/customer/1
        // Get all reviews by a customer
        // ==================================================
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReviewDto>>>> GetByCustomer(int customerId)
        {
            var reviews = await _reviewService.GetByCustomerAsync(customerId);
            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(reviews, "Customer reviews retrieved successfully"));
        }

        // ==================================================
        // GET api/review/product/1/rating
        // Get average rating for a product
        // ==================================================
        [HttpGet("product/{productId}/rating")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<decimal>>> GetAverageRating(int productId)
        {
            var rating = await _reviewService.GetAverageRatingAsync(productId);
            return Ok(ApiResponse<decimal>.Ok(rating, "Average rating retrieved successfully"));
        }

        // ==================================================
        // POST api/review
        // Submit a review
        // ==================================================
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> Create(ReviewCreateDto createDto)
        {
            // Check if customer already reviewed this product
            if (await _reviewService.ReviewExistsAsync(createDto.ProductID, createDto.CustomerID))
                return BadRequest(ApiResponse<ReviewDto>.Fail("You have already reviewed this product"));

            // Validate rating range
            if (createDto.Rating < 1.0m || createDto.Rating > 5.0m)
                return BadRequest(ApiResponse<ReviewDto>.Fail("Rating must be between 1.0 and 5.0"));

            var review = await _reviewService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = review.ReviewID },
                                   ApiResponse<ReviewDto>.Ok(review, "Review submitted successfully"));
        }

        // ==================================================
        // PUT api/review/1
        // Update a review
        // ==================================================
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> Update(int id, ReviewUpdateDto updateDto)
        {
            // Validate rating range
            if (updateDto.Rating < 1.0m || updateDto.Rating > 5.0m)
                return BadRequest(ApiResponse<ReviewDto>.Fail("Rating must be between 1.0 and 5.0"));

            var review = await _reviewService.UpdateAsync(id, updateDto);
            if (review == null)
                return NotFound(ApiResponse<ReviewDto>.Fail($"Review with ID {id} was not found"));

            return Ok(ApiResponse<ReviewDto>.Ok(review, "Review updated successfully"));
        }

        // ==================================================
        // DELETE api/review/1
        // ==================================================
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _reviewService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Review with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Review deleted successfully"));
        }
    }
}
