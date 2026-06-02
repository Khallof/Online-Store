using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Repositories
{
    public interface IShippingRepository : IGenericRepository<Shipping>
    {
        // Get shipping by order ID (1-to-1 with order)
        Task<Shipping?> GetByOrderAsync(int orderId);

        // Get shipping by tracking number
        Task<Shipping?> GetByTrackingNumberAsync(string trackingNumber);

        // Get all shippings by status
        Task<IEnumerable<Shipping>> GetByStatusAsync(short shippingStatus);

        // Check if an order already has a shipping record
        Task<bool> OrderHasShippingAsync(int orderId);

        // Update shipping status and delivery dates
        Task UpdateStatusAsync(int shippingId, short status, DateTime? actualDeliveryDate);
    }
}
