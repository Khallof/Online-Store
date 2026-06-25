using Store.Core.DTOs.Shipping;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services
{
    public class ShippingService : IShippingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShippingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================
        // Mapping Methods
        // ==================================================
        private ShippingDto MapToDto(Shipping shipping)
        {
            return new ShippingDto
            {
                ShippingID = shipping.ShippingID,
                OrderID = shipping.OrderID,
                CarrierName = shipping.CarrierName,
                TrackingNumber = shipping.TrackingNumber,
                Status = shipping.ShippingStatus.ToString(),
                EstimatedDeliveryDate = shipping.EstimatedDeliveryDate,
                ActualDeliveryDate = shipping.ActualDeliveryDate
            };
        }

        private Shipping MapToEntity(ShippingCreateDto dto)
        {
            return new Shipping
            {
                OrderID = dto.OrderID,
                CarrierName = dto.CarrierName,
                TrackingNumber = dto.TrackingNumber,
                EstimatedDeliveryDate = dto.EstimatedDeliveryDate,
                ShippingStatus = 0  // Pending by default
            };
        }

        // ==================================================
        // IGenericService Implementation
        // ==================================================
        public async Task<IEnumerable<ShippingDto>> GetAllAsync()
        {
            var shippings = await _unitOfWork.Shippings.GetAllAsync();
            return shippings.Select(s => MapToDto(s));
        }

        public async Task<ShippingDto?> GetByIdAsync(int id)
        {
            var shipping = await _unitOfWork.Shippings.GetByIdAsync(id);
            if (shipping == null) return null;
            return MapToDto(shipping);
        }

        public async Task<ShippingDto> CreateAsync(ShippingCreateDto createDto)
        {
            // Check if order already has a shipping record
            var alreadyExists = await _unitOfWork.Shippings.OrderHasShippingAsync(createDto.OrderID);


            if (alreadyExists)
                throw new InvalidOperationException($"Order {createDto.OrderID} already has a shipping record.");

            var order = await _unitOfWork.Orders.GetByIdAsync(createDto.OrderID);

            if (order == null)
                throw new InvalidOperationException(
                    $"Order with ID {createDto.OrderID} does not exist.");

            if (order.Status==4) 
                throw new InvalidOperationException($"Order {createDto.OrderID} is already cancelled.");

            if (createDto.EstimatedDeliveryDate.HasValue &&
              createDto.EstimatedDeliveryDate.Value <= order.Orderdate)
                throw new InvalidOperationException(
                    "Estimated delivery date must be after the order date " +
                    $"({order.Orderdate:yyyy-MM-dd}).");

            var shipping = MapToEntity(createDto);
            await _unitOfWork.Shippings.AddAsync(shipping);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(shipping);
        }

        public async Task<ShippingDto?> UpdateAsync(int id, ShippingUpdateDto updateDto)
        {
            var shipping = await _unitOfWork.Shippings.GetByIdAsync(id);
            if (shipping == null) return null;

            var order = await _unitOfWork.Orders.GetByIdAsync(shipping.OrderID);

            if (updateDto.ActualDeliveryDate.HasValue && order != null &&
              updateDto.ActualDeliveryDate.Value < order.Orderdate)
                throw new InvalidOperationException(
                    $"Actual delivery date ({updateDto.ActualDeliveryDate.Value:yyyy-MM-dd}) " +
                    $"cannot be before the order date ({order.Orderdate:yyyy-MM-dd}).");


            shipping.TrackingNumber = updateDto.TrackingNumber;
            shipping.ShippingStatus = updateDto.ShippingStatus;
            shipping.EstimatedDeliveryDate = updateDto.EstimatedDeliveryDate;
            shipping.ActualDeliveryDate = updateDto.ActualDeliveryDate;

            _unitOfWork.Shippings.Update(shipping);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(shipping);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var shipping = await _unitOfWork.Shippings.GetByIdAsync(id);
            if (shipping == null) return false;

            _unitOfWork.Shippings.Delete(shipping);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // IShippingService Specific Methods
        // ==================================================
        public async Task<ShippingDto?> GetByOrderAsync(int orderId)
        {
            var shipping = await _unitOfWork.Shippings.GetByOrderAsync(orderId);
            if (shipping == null) return null;
            return MapToDto(shipping);
        }

        public async Task<ShippingDto?> GetByTrackingNumberAsync(string trackingNumber)
        {
            var shipping = await _unitOfWork.Shippings.GetByTrackingNumberAsync(trackingNumber);
            if (shipping == null) return null;
            return MapToDto(shipping);
        }

        public async Task<IEnumerable<ShippingDto>> GetByStatusAsync(short shippingStatus)
        {
            var shippings = await _unitOfWork.Shippings.GetByStatusAsync(shippingStatus);
            return shippings.Select(s => MapToDto(s));
        }

        public async Task<bool> UpdateStatusAsync(int shippingId, short status, DateTime? actualDeliveryDate)
        {
            await _unitOfWork.Shippings.UpdateStatusAsync(shippingId, status, actualDeliveryDate);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> OrderHasShippingAsync(int orderId)
        {
            return await _unitOfWork.Shippings.OrderHasShippingAsync(orderId);
        }

       

    }
}
