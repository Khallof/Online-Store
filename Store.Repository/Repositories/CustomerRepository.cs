using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;

namespace Store.Repository.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers
                                 .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await _context.Customers
                                 .FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<Customer?> GetWithOrdersAsync(int customerId)
        {
            return await _context.Customers
                                 .Include(c => c.Order)
                                 .FirstOrDefaultAsync(c => c.CustomerID == customerId);
        }

        public async Task<Customer?> GetWithReviewsAsync(int customerId)
        {
            return await _context.Customers
                                 .Include(c => c.Review)
                                 .FirstOrDefaultAsync(c => c.CustomerID == customerId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Customers
                                 .AnyAsync(c => c.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Customers
                                 .AnyAsync(c => c.Username == username);
        }
    }
}
