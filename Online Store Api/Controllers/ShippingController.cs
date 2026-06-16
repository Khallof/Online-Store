using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Store.API.Helpers;
using Store.Core.DTOs.Payment;
using Store.Core.DTOs.Shipping;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        private bool IsAdminOrSameCustomer(int customerId)
        {
            var loggedInId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var loggedInRole = User.FindFirst("role")?.Value;
            return loggedInRole == "Admin" || loggedInId == customerId;
        }

        // ==================================================
        // GET api/shipping
        // ==================================================
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShippingDto>>>> GetAll()
        {
            var shippings = await _shippingService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ShippingDto>>.Ok(shippings, "Shippings retrieved successfully"));
        }

        // ==================================================
        // GET api/shipping/1
        // ==================================================
        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ShippingDto>>> GetById(int id)
        {
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<ShippingDto>.Fail("You can only view your own data"));
            var shipping = await _shippingService.GetByIdAsync(id);
            if (shipping == null)
                return NotFound(ApiResponse<ShippingDto>.Fail($"Shipping with ID {id} was not found"));

            return Ok(ApiResponse<ShippingDto>.Ok(shipping, "Shipping retrieved successfully"));
        }

        // ==================================================
        // GET api/shipping/order/1
        // Get shipping by order ID
        // ==================================================
        [HttpGet("order/{orderId}")]
        [Authorize(Policy ="AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ShippingDto>>> GetByOrder(int orderId)
        {
            if (!IsAdminOrSameCustomer(orderId))
                return Unauthorized(ApiResponse<ShippingDto>.Fail("You can only view your own data"));
            var shipping = await _shippingService.GetByOrderAsync(orderId);
            if (shipping == null)
                return NotFound(ApiResponse<ShippingDto>.Fail($"No shipping found for order {orderId}"));

            return Ok(ApiResponse<ShippingDto>.Ok(shipping, "Shipping retrieved successfully"));
        }

        // ==================================================
        // GET api/shipping/track/FX100001
        // Get shipping by tracking number
        // ==================================================
        [HttpGet("track/{trackingNumber}")]
        [Authorize(Policy ="AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ShippingDto>>> GetByTrackingNumber(string trackingNumber)
        {
            var shipping = await _shippingService.GetByTrackingNumberAsync(trackingNumber);
            if (shipping == null)
                return NotFound(ApiResponse<ShippingDto>.Fail($"No shipping found with tracking number {trackingNumber}"));

            return Ok(ApiResponse<ShippingDto>.Ok(shipping, "Shipping retrieved successfully"));
        }

        // ==================================================
        // GET api/shipping/status/1
        // Get shippings by status
        // ==================================================
        [HttpGet("status/{status}")]
        [Authorize(Policy ="AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ShippingDto>>>> GetByStatus(short status)
        {
            var shippings = await _shippingService.GetByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<ShippingDto>>.Ok(shippings, "Shippings retrieved successfully"));
        }

        // ==================================================
        // POST api/shipping
        // Create a shipping record for an order
        // ==================================================
        [HttpPost]
        [AllowAnonymous]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ShippingDto>>> Create(ShippingCreateDto createDto)
        {
            // Check if order already has a shipping record
            if (await _shippingService.OrderHasShippingAsync(createDto.OrderID))
                return BadRequest(ApiResponse<ShippingDto>.Fail($"Order {createDto.OrderID} already has a shipping record"));

            var shipping = await _shippingService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = shipping.ShippingID },
                                   ApiResponse<ShippingDto>.Ok(shipping, "Shipping created successfully"));
        }

        // ==================================================
        // PUT api/shipping/1
        // Update shipping info
        // ==================================================
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ShippingDto>>> Update(int id, ShippingUpdateDto updateDto)
        {
            var shipping = await _shippingService.UpdateAsync(id, updateDto);
            if (shipping == null)
                return NotFound(ApiResponse<ShippingDto>.Fail($"Shipping with ID {id} was not found"));

            return Ok(ApiResponse<ShippingDto>.Ok(shipping, "Shipping updated successfully"));
        }

        // ==================================================
        // PUT api/shipping/1/status
        // Update shipping status and delivery date
        // ==================================================
        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(
            int id,
            [FromQuery] short status,
            [FromQuery] DateTime? actualDeliveryDate)
        {
            var result = await _shippingService.UpdateStatusAsync(id, status, actualDeliveryDate);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Shipping with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Shipping status updated successfully"));
        }

        // ==================================================
        // DELETE api/shipping/1
        // ==================================================
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _shippingService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Shipping with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Shipping deleted successfully"));
        }
    }
}
