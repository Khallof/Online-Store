using Microsoft.AspNetCore.Mvc;
using Store.API.Helpers;
using Store.Core.DTOs.Payment;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ==================================================
        // GET api/payment
        // ==================================================
        [HttpGet]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> GetById(int id)
        {
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> GetByOrder(int orderId)
        {
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaymentDto>>> Create(PaymentCreateDto createDto)
        {
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
