using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Store.API.Helpers;
using Store.Core.DTOs.Customer;
using Store.Core.DTOs.Order;
using Store.Core.Interfaces.Services;
using System.Net.NetworkInformation;

namespace Store.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }



        private bool IsAdminOrSameCustomer(int customerId)
        {
            var loggedInId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var loggedInRole = User.FindFirst("role")?.Value;
            return loggedInRole == "Admin" || loggedInId == customerId;
        }



        // ==================================================
        // GET api/order
        // ==================================================
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderSummaryDto>>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<OrderSummaryDto>>.Ok(orders, "Orders retrieved successfully"));
        }

        // ==================================================
        // GET api/order/1
        // ==================================================
        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderSummaryDto>>> GetById(int id)
        {
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<OrderSummaryDto>.Fail("You can only view your own data"));
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<OrderSummaryDto>.Fail($"Order with ID {id} was not found"));

            return Ok(ApiResponse<OrderSummaryDto>.Ok(order, "Order retrieved successfully"));
        }

        // ==================================================
        // GET api/order/1/details
        // Get full order detail with items, payment, shipping
        // ==================================================
        [HttpGet("{id}/details")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetDetail(int id)
        {
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<OrderDetailDto>.Fail("You can only view your own data"));
            var order = await _orderService.GetDetailByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<OrderDetailDto>.Fail($"Order with ID {id} was not found"));

            return Ok(ApiResponse<OrderDetailDto>.Ok(order, "Order details retrieved successfully"));
        }

        // ==================================================
        // GET api/order/customer/1
        // Get all orders for a customer
        // ==================================================
        [HttpGet("customer/{customerId}")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderSummaryDto>>>> GetByCustomer(int customerId)
        {
            if (!IsAdminOrSameCustomer(customerId))
                return Unauthorized(ApiResponse<IEnumerable<OrderSummaryDto>>.Fail("You can only view your own orders"));

            var orders = await _orderService.GetByCustomerAsync(customerId);
            return Ok(ApiResponse<IEnumerable<OrderSummaryDto>>.Ok(orders, "Customer orders retrieved successfully"));
        }

        // ==================================================
        // GET api/order/status/0
        // Get orders by status
        // ==================================================
        [HttpGet("status/{status}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderSummaryDto>>>> GetByStatus(short status)
        {
            if(!IsAdminOrSameCustomer(status))
                return Unauthorized(ApiResponse<IEnumerable<OrderSummaryDto>>.Fail("You can only view your own orders"));
            var orders = await _orderService.GetByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<OrderSummaryDto>>.Ok(orders, "Orders retrieved successfully"));
        }

        // ==================================================
        // GET api/order/daterange?start=2024-01-01&end=2024-12-31
        // ==================================================
        [HttpGet("daterange")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderSummaryDto>>>> GetByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var orders = await _orderService.GetByDateRangeAsync(start, end);
            return Ok(ApiResponse<IEnumerable<OrderSummaryDto>>.Ok(orders, "Orders retrieved successfully"));
        }

        // ==================================================
        // POST api/order
        // Place a new order
        // ==================================================
        [AllowAnonymous]
        [HttpPost]
        [EnableRateLimiting("WritePolicy")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<OrderSummaryDto>>> Create(OrderCreateDto createDto)
        {
            var order = await _orderService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = order.OrderID },
                                   ApiResponse<OrderSummaryDto>.Ok(order, "Order placed successfully"));
        }

        // ==================================================
        // PUT api/order/1/status
        // Update order status only
        // ==================================================
        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, OrderUpdateStatusDto updateDto)
        {
            var result = await _orderService.UpdateStatusAsync(id, updateDto.Status);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Order with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Order status updated successfully"));
        }

        // ==================================================
        // PUT api/order/1/cancel
        // Cancel an order
        // ==================================================
        [HttpPut("{id}/cancel")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("WritePolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Cancel(int id)
        {
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<IEnumerable<bool>>.Fail("You can only view your own orders"));

            var result = await _orderService.CancelOrderAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Order with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Order cancelled successfully"));
        }

        // ==================================================
        // DELETE api/order/1
        // ==================================================
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _orderService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Order with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Order deleted successfully"));
        }
    }
}
