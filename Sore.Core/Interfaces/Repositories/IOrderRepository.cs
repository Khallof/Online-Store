using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Get all orders with customer, items, payment, shipping
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();

        // Get a single order with all details
        Task<Order?> GetWithDetailsAsync(int orderId);

        // Get all orders for a specific customer
        Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);

        // Get orders by status (0=Pending, 1=Processing, 2=Shipped, 3=Delivered, 4=Cancelled)
        Task<IEnumerable<Order>> GetByStatusAsync(short status);

        // Get orders placed between two dates
        Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Update order status only
        Task UpdateStatusAsync(int orderId, short status);
    }
}
