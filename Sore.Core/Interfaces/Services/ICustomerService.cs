using Store.Core.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface ICustomerService : IGenericService<CustomerDto, CustomerCreateDto, CustomerUpdateDto>
    {
        // Get customer by email (for login)
        Task<CustomerDto?> GetByEmailAsync(string email);

        // Get customer by username
        Task<CustomerDto?> GetByUsernameAsync(string username);

        // Get customer with all their orders
        Task<CustomerDto?> GetWithOrdersAsync(int customerId);

        // Check if email is already taken
        Task<bool> EmailExistsAsync(string email);

        // Check if username is already taken
        Task<bool> UsernameExistsAsync(string username);
    }
}
