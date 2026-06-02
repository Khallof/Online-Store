using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        // Get payment by order ID (1-to-1 with order)
        Task<Payment?> GetByOrderAsync(int orderId);

        // Get payments by method (e.g. all PayPal payments)
        Task<IEnumerable<Payment>> GetByMethodAsync(string paymentMethod);

        // Get payments between two dates
        Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Check if an order already has a payment
        Task<bool> OrderHasPaymentAsync(int orderId);
    }
}
