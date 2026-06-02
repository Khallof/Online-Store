using Microsoft.AspNetCore.Mvc;
using Store.API.Helpers;
using Store.Core.DTOs.Order;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // ==================================================
        // GET api/order
        // ==================================================
        [HttpGet]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderSummaryDto>>> GetById(int id)
        {
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetDetail(int id)
        {
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderSummaryDto>>>> GetByCustomer(int customerId)
        {
            var orders = await _orderService.GetByCustomerAsync(customerId);
            return Ok(ApiResponse<IEnumerable<OrderSummaryDto>>.Ok(orders, "Customer orders retrieved successfully"));
        }

        // ==================================================
        // GET api/order/status/0
        // Get orders by status
        // ==================================================
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderSummaryDto>>>> GetByStatus(short status)
        {
            var orders = await _orderService.GetByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<OrderSummaryDto>>.Ok(orders, "Orders retrieved successfully"));
        }

        // ==================================================
        // GET api/order/daterange?start=2024-01-01&end=2024-12-31
        // ==================================================
        [HttpGet("daterange")]
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
        [HttpPost]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Cancel(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Order with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Order cancelled successfully"));
        }

        // ==================================================
        // DELETE api/order/1
        // ==================================================
        [HttpDelete("{id}")]
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
