using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;

namespace Store.Repository.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
        {
            return await _context.Orders
                                 .Include(o => o.Customer)
                                 .Include(o => o.OrderItem)
                                 .ThenInclude(oi => oi.ProductCatalog)
                                 .Include(o => o.Payment)
                                 .Include(o => o.Shipping)
                                 .ToListAsync();
        }

        public async Task<Order?> GetWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                                 .Include(o => o.Customer)
                                 .Include(o => o.OrderItem)
                                 .ThenInclude(oi => oi.ProductCatalog)
                                 .ThenInclude(p => p.ProductImages)
                                 .Include(o => o.Payment)
                                 .Include(o => o.Shipping)
                                 .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
        {
            return await _context.Orders
                                 .Include(o => o.OrderItem)
                                 .Where(o => o.CustomerID == customerId)
                                 .OrderByDescending(o => o.Orderdate)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(short status)
        {
            return await _context.Orders
                                 .Include(o => o.Customer)
                                 .Where(o => o.Status == status)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                                 .Include(o => o.Customer)
                                 .Where(o => o.Orderdate >= startDate && o.Orderdate <= endDate)
                                 .OrderByDescending(o => o.Orderdate)
                                 .ToListAsync();
        }

        public async Task UpdateStatusAsync(int orderId, short status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Orders.Update(order);
            }
        }
    }
}
