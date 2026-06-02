using Microsoft.AspNetCore.Mvc;
using Store.API.Helpers;
using Store.Core.DTOs.Customer;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
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
        // GET api/customer
        // Get all customers
        // ==================================================
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerDto>>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CustomerDto>>.Ok(customers, "Customers retrieved successfully"));
        }

        // ==================================================
        // GET api/customer/1
        // Get a single customer by ID
        // ==================================================
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with ID {id} was not found"));

            return Ok(ApiResponse<CustomerDto>.Ok(customer, "Customer retrieved successfully"));
        }

        // ==================================================
        // GET api/customer/email/alice@example.com
        // Get customer by email
        // ==================================================
        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetByEmail(string email)
        {
            var customer = await _customerService.GetByEmailAsync(email);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with email {email} was not found"));

            return Ok(ApiResponse<CustomerDto>.Ok(customer, "Customer retrieved successfully"));
        }

        // ==================================================
        // GET api/customer/1/orders
        // Get customer with their orders
        // ==================================================
        [HttpGet("{id}/orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetWithOrders(int id)
        {
            var customer = await _customerService.GetWithOrdersAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with ID {id} was not found"));

            return Ok(ApiResponse<CustomerDto>.Ok(customer, "Customer with orders retrieved successfully"));
        }

        // ==================================================
        // POST api/customer
        // Create a new customer
        // ==================================================
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Create(CustomerCreateDto createDto)
        {
            // Business validation
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
        // Update an existing customer
        // ==================================================
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Update(int id, CustomerUpdateDto updateDto)
        {
            var customer = await _customerService.UpdateAsync(id, updateDto);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with ID {id} was not found"));

            return Ok(ApiResponse<CustomerDto>.Ok(customer, "Customer updated successfully"));
        }

        // ==================================================
        // DELETE api/customer/1
        // Delete a customer
        // ==================================================
        [HttpDelete("{id}")]
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
