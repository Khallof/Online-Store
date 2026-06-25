using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Store.API.Helpers;
using Store.Core.DTOs.Order;
using Store.Core.DTOs.Payment;
using Store.Core.Entities;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        private bool IsAdminOrSameCustomer(int customerId)
        {
            var loggedInId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var loggedInRole = User.FindFirst("role")?.Value;
            return loggedInRole == "Admin" || loggedInId == customerId;
        }


        // ==================================================
        // GET api/payment
        // ==================================================
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentDto>>>> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<PaymentDto>>.Ok(payments, "Payments retrieved successfully"));
        }

        // ==================================================
        // GET api/payment/1
        // ==================================================
        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> GetById(int id)
        {
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<PaymentDto>.Fail("You can only view your own data"));
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
                return NotFound(ApiResponse<PaymentDto>.Fail($"Payment with ID {id} was not found"));

            return Ok(ApiResponse<PaymentDto>.Ok(payment, "Payment retrieved successfully"));
        }

        // ==================================================
        // GET api/payment/order/1
        // Get payment by order ID
        // ==================================================
        [HttpGet("order/{orderId}")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("ReadPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> GetByOrder(int orderId)
        {
            if (!IsAdminOrSameCustomer(orderId))
                return Unauthorized(ApiResponse<PaymentDto>.Fail("You can only view your own data"));
            var payment = await _paymentService.GetByOrderAsync(orderId);
            if (payment == null)
                return NotFound(ApiResponse<PaymentDto>.Fail($"No payment found for order {orderId}"));

            return Ok(ApiResponse<PaymentDto>.Ok(payment, "Payment retrieved successfully"));
        }

        // ==================================================
        // GET api/payment/method/CreditCard
        // Get payments by method
        // ==================================================
        [HttpGet("method/{method}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentDto>>>> GetByMethod(string method)
        {
            var payments = await _paymentService.GetByMethodAsync(method);
            return Ok(ApiResponse<IEnumerable<PaymentDto>>.Ok(payments, "Payments retrieved successfully"));
        }

        // ==================================================
        // GET api/payment/daterange?start=2024-01-01&end=2024-12-31
        // ==================================================
        [HttpGet("daterange")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentDto>>>> GetByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var payments = await _paymentService.GetByDateRangeAsync(start, end);
            return Ok(ApiResponse<IEnumerable<PaymentDto>>.Ok(payments, "Payments retrieved successfully"));
        }

        // ==================================================
        // POST api/payment
        // Create a payment for an order
        // ==================================================
        [HttpPost]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("WritePolicy")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> Create(PaymentCreateDto createDto)
        {
            if (!IsAdminOrSameCustomer(createDto.Customer.CustomerID))
                return Unauthorized(ApiResponse<PaymentDto>.Fail("You can only Creat Payment For your own Orders"));

            // Check if order already has a payment
            if (await _paymentService.OrderHasPaymentAsync(createDto.OrderID))
                return BadRequest(ApiResponse<PaymentDto>.Fail($"Order {createDto.OrderID} already has a payment"));

            var payment = await _paymentService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = payment.PaymentID },
                                   ApiResponse<PaymentDto>.Ok(payment, "Payment created successfully"));
        }

        // ==================================================
        // DELETE api/payment/1
        // ==================================================
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _paymentService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Payment with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Payment deleted successfully"));
        }
    }
}
