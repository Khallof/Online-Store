using Store.Core.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface IPaymentService : IGenericService<PaymentDto, PaymentCreateDto, PaymentCreateDto>
    {
        // Get payment by order ID (1-to-1 with order)
        Task<PaymentDto?> GetByOrderAsync(int orderId);

        // Get payments by method (e.g. all PayPal payments)
        Task<IEnumerable<PaymentDto>> GetByMethodAsync(string paymentMethod);

        // Get payments between two dates
        Task<IEnumerable<PaymentDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Check if an order already has a payment
        Task<bool> OrderHasPaymentAsync(int orderId);
    }
}
