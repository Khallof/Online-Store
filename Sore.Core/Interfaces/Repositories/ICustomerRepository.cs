using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{

    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        // Get customer by email (for login / duplicate check)
        Task<Customer?> GetByEmailAsync(string email);

        // Get customer by username (for duplicate check)
        Task<Customer?> GetByUsernameAsync(string username);

        // Get customer with their orders
        Task<Customer?> GetWithOrdersAsync(int customerId);

        // Get customer with their reviews
        Task<Customer?> GetWithReviewsAsync(int customerId);

        // Check if email already exists (for validation)
        Task<bool> EmailExistsAsync(string email);

        // Check if username already exists (for validation)
        Task<bool> UsernameExistsAsync(string username);
    }

}
