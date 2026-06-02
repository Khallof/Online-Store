using Store.Core.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface IOrderService : IGenericService<OrderSummaryDto, OrderCreateDto, OrderUpdateStatusDto>
    {
        // Get full order detail with items, payment, shipping
        Task<OrderDetailDto?> GetDetailByIdAsync(int orderId);

        // Get all orders for a specific customer
        Task<IEnumerable<OrderSummaryDto>> GetByCustomerAsync(int customerId);

        // Get orders by status
        Task<IEnumerable<OrderSummaryDto>> GetByStatusAsync(short status);

        // Get orders between two dates
        Task<IEnumerable<OrderSummaryDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Update order status only
        Task<bool> UpdateStatusAsync(int orderId, short status);

        // Cancel an order
        Task<bool> CancelOrderAsync(int orderId);
    }
}
