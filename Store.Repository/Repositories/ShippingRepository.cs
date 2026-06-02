using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;

namespace Store.Repository.Repositories
{
    public class ShippingRepository : GenericRepository<Shipping>, IShippingRepository
    {
        public ShippingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Shipping?> GetByOrderAsync(int orderId)
        {
            return await _context.Shippings
                                 .FirstOrDefaultAsync(s => s.OrderID == orderId);
        }

        public async Task<Shipping?> GetByTrackingNumberAsync(string trackingNumber)
        {
            return await _context.Shippings
                                 .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
        }

        public async Task<IEnumerable<Shipping>> GetByStatusAsync(short shippingStatus)
        {
            return await _context.Shippings
                                 .Where(s => s.ShippingStatus == shippingStatus)
                                 .ToListAsync();
        }

        public async Task<bool> OrderHasShippingAsync(int orderId)
        {
            return await _context.Shippings
                                 .AnyAsync(s => s.OrderID == orderId);
        }

        public async Task UpdateStatusAsync(int shippingId, short status, DateTime? actualDeliveryDate)
        {
            var shipping = await _context.Shippings.FindAsync(shippingId);
            if (shipping != null)
            {
                shipping.ShippingStatus = status;
                shipping.ActualDeliveryDate = actualDeliveryDate;
                _context.Shippings.Update(shipping);
            }
        }
    }
}
