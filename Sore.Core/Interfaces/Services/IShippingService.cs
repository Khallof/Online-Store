using Store.Core.DTOs.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface IShippingService : IGenericService<ShippingDto, ShippingCreateDto, ShippingUpdateDto>
    {
        // Get shipping by order ID (1-to-1 with order)
        Task<ShippingDto?> GetByOrderAsync(int orderId);

        // Get shipping by tracking number
        Task<ShippingDto?> GetByTrackingNumberAsync(string trackingNumber);

        // Get all shippings by status
        Task<IEnumerable<ShippingDto>> GetByStatusAsync(short shippingStatus);

        // Update shipping status and delivery dates
        Task<bool> UpdateStatusAsync(int shippingId, short status, DateTime? actualDeliveryDate);

        // Check if an order already has a shipping record
        Task<bool> OrderHasShippingAsync(int orderId);

       
    }
}
