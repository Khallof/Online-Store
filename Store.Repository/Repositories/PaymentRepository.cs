using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;

namespace Store.Repository.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Payment?> GetByOrderAsync(int orderId)
        {
            return await _context.Payments
                                 .FirstOrDefaultAsync(p => p.OrderID == orderId);
        }

        public async Task<IEnumerable<Payment>> GetByMethodAsync(string paymentMethod)
        {
            return await _context.Payments
                                 .Where(p => p.PaymentMethod == paymentMethod)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                                 .Where(p => p.TransactionDate >= startDate &&
                                             p.TransactionDate <= endDate)
                                 .OrderByDescending(p => p.TransactionDate)
                                 .ToListAsync();
        }

        public async Task<bool> OrderHasPaymentAsync(int orderId)
        {
            return await _context.Payments
                                 .AnyAsync(p => p.OrderID == orderId);
        }
    }
}
