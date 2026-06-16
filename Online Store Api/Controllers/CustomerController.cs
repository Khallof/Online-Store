using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Helpers;
using Store.Core.DTOs.Customer;
using Store.Core.Interfaces.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Store.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // ==================================================
        // Helper — checks if logged in user is Admin
        //          OR is the same customer as requested
        // ==================================================
        private bool IsAdminOrSameCustomer(int customerId)
        {
            var loggedInId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var loggedInRole = User.FindFirst("role")?.Value;
            return loggedInRole == "Admin" || loggedInId == customerId;
        }


     
        // ==================================================
        // GET api/customer
        // ==================================================
        [HttpGet]
        [EnableRateLimiting("ReadPolicy")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerDto>>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CustomerDto>>.Ok(customers, "Customers retrieved successfully"));
        }

        // ==================================================
        // GET api/customer/1
        // ==================================================
        [HttpGet("{id}")]
        [EnableRateLimiting("ReadPolicy")]
        [Authorize(Policy = "AllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetById(int id)
        {
            //  Check if Admin or same customer
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<CustomerDto>.Fail("You can only view your own data"));

            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with ID {id} was not found"));

            return Ok(ApiResponse<CustomerDto>.Ok(customer, "Customer retrieved successfully"));
        }

        // ==================================================
        // GET api/customer/email/alice@example.com
        // ==================================================
        [HttpGet("email/{email}")]
        [EnableRateLimiting("ReadPolicy")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetByEmail(string email)
        {
           ;
            var customer = await _customerService.GetByEmailAsync(email);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with email {email} was not found"));

            return Ok(ApiResponse<CustomerDto>.Ok(customer, "Customer retrieved successfully"));
        }

        // ==================================================
        // GET api/customer/1/orders
        // ==================================================
        [HttpGet("{id}/orders")]
        [EnableRateLimiting("ReadPolicy")]
        [Authorize(Policy = "AllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetWithOrders(int id)
        {
            
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<CustomerDto>.Fail("You can only view your own orders"));

            var customer = await _customerService.GetWithOrdersAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with ID {id} was not found"));

            return Ok(ApiResponse<CustomerDto>.Ok(customer, "Customer with orders retrieved successfully"));
        }

        // ==================================================
        // POST api/customer
        // ==================================================
        [AllowAnonymous]
        [HttpPost]
        [EnableRateLimiting("WritePolicy")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Create(CustomerCreateDto createDto)
        {
            if (await _customerService.EmailExistsAsync(createDto.Email))
                return BadRequest(ApiResponse<CustomerDto>.Fail("Email already exists"));

            if (await _customerService.UsernameExistsAsync(createDto.Username))
                return BadRequest(ApiResponse<CustomerDto>.Fail("Username already exists"));

            var customer = await _customerService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = customer.CustomerID },
                                   ApiResponse<CustomerDto>.Ok(customer, "Customer created successfully"));
        }

        // ==================================================
        // PUT api/customer/1
        // 🔒 Admin OR same customer only
        // ==================================================
        [HttpPut("{id}")]
        [Authorize(Policy = "AllUsers")]
        [EnableRateLimiting("WritePolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Update(int id, CustomerUpdateDto updateDto)
        {
            // ✅ Check if Admin or same customer
            if (!IsAdminOrSameCustomer(id))
                return Unauthorized(ApiResponse<CustomerDto>.Fail("You can only update your own data"));

            var customer = await _customerService.UpdateAsync(id, updateDto);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with ID {id} was not found"));

            return Ok(ApiResponse<CustomerDto>.Ok(customer, "Customer updated successfully"));
        }

        // ==================================================
        // DELETE api/customer/1
        // 🔒 Admin only
        // ==================================================
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [DisableRateLimiting]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.Fail($"Customer with ID {id} was not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Customer deleted successfully"));
        }
    }
}