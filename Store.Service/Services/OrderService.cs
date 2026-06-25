using Store.Core.DTOs.Order;
using Store.Core.DTOs.Payment;
using Store.Core.DTOs.Shipping;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================
        // Mapping Methods
        // ==================================================
        private OrderSummaryDto MapToSummaryDto(Order order)
        {
            return new OrderSummaryDto
            {
                OrderID = order.OrderID,
                OrderDate = order.Orderdate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ItemCount = order.OrderItem?.Count ?? 0
            };
        }

        private OrderDetailDto MapToDetailDto(Order order)
        {
            return new OrderDetailDto
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                CustomerName = order.Customer?.Name ?? string.Empty,
                OrderDate = order.Orderdate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.OrderItem?.Select(oi => new OrderItemDto
                {
                    OrderItemID = oi.OrderItemID,
                    ProductID = oi.ProductID,
                    ProductName = oi.ProductCatalog?.ProductName ?? string.Empty,
                    ThumbnailUrl = oi.ProductCatalog?.ProductImages
                                       .OrderBy(i => i.ImageOrder)
                                       .FirstOrDefault()?.ImageURL,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    TotalItemsPrice = oi.TotalItemsPrice
                }).ToList() ?? new List<OrderItemDto>(),

                Payment = order.Payment == null ? null : new PaymentDto
                {
                    PaymentID = order.Payment.PaymentID,
                    OrderID = order.Payment.OrderID,
                    Amount = order.Payment.Amount,
                    PaymentMethod = order.Payment.PaymentMethod,
                    TransactionDate = order.Payment.TransactionDate
                },

                Shipping = order.Shipping == null ? null : new ShippingDto
                {
                    ShippingID = order.Shipping.ShippingID,
                    OrderID = order.Shipping.OrderID,
                    CarrierName = order.Shipping.CarrierName,
                    TrackingNumber = order.Shipping.TrackingNumber,
                    Status = order.Shipping.ShippingStatus.ToString(),
                    EstimatedDeliveryDate = order.Shipping.EstimatedDeliveryDate,
                    ActualDeliveryDate = order.Shipping.ActualDeliveryDate
                }
            };
        }

        // ==================================================
        // IGenericService Implementation
        // ==================================================
        public async Task<IEnumerable<OrderSummaryDto>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync();
            return orders.Select(o => MapToSummaryDto(o));
        }

        public async Task<OrderSummaryDto?> GetByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return null;
            return MapToSummaryDto(order);
        }

        public async Task<OrderSummaryDto> CreateAsync(OrderCreateDto createDto)
        {
            // 1 - Calculate total amount from product prices
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();
            var validationErrors = new List<string>();


            foreach (var item in createDto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductID);
                if (product == null)
                {
                    validationErrors.Add($"Product with ID {item.ProductID} does not exist.");
                    continue;
                }

                if (product.QuantityInStock < item.Quantity)
                {
                    validationErrors.Add(
                        $"'{product.ProductName}' only has {product.QuantityInStock} " +
                        $"items in stock but you requested {item.Quantity}.");
                    continue;
                }

                if (product.QuantityInStock - item.Quantity < 0)
                {
                    validationErrors.Add($"'{product.ProductName}' stock cannot go below 0.");
                    continue;
                }


                var orderItem = new OrderItem
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    TotalItemsPrice = product.Price * item.Quantity
                };

                totalAmount += orderItem.TotalItemsPrice;
                orderItems.Add(orderItem);

                // 2 - Update stock
                await _unitOfWork.Products.UpdateStockAsync(item.ProductID, item.Quantity);
            }
            if (validationErrors.Any())
                throw new InvalidOperationException(string.Join(" | ", validationErrors));

            // 3 - Create order
            var order = new Order
            {
                CustomerID = createDto.CustomerID,
                Orderdate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = 0,  // Pending
                OrderItem = orderItems
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return MapToSummaryDto(order);
        }

        public async Task<OrderSummaryDto?> UpdateAsync(int id, OrderUpdateStatusDto updateDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return null;

            var currentStatus = order.Status;
            var newStatus = updateDto.Status;

            if (newStatus != 4 && newStatus < currentStatus)
                throw new InvalidOperationException(
                    $"Cannot change order status from '{GetStatusLabel(currentStatus)}' " +
                    $"back to '{GetStatusLabel(newStatus)}'. Status can only move forward.");




            order.Status = updateDto.Status;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return MapToSummaryDto(order);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return false;

            _unitOfWork.Orders.Delete(order);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // IOrderService Specific Methods
        // ==================================================
        public async Task<OrderDetailDto?> GetDetailByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetWithDetailsAsync(orderId);
            if (order == null) return null;
            return MapToDetailDto(order);
        }

        public async Task<IEnumerable<OrderSummaryDto>> GetByCustomerAsync(int customerId)
        {
            var orders = await _unitOfWork.Orders.GetByCustomerAsync(customerId);
            return orders.Select(o => MapToSummaryDto(o));
        }

        public async Task<IEnumerable<OrderSummaryDto>> GetByStatusAsync(short status)
        {
            var orders = await _unitOfWork.Orders.GetByStatusAsync(status);
            return orders.Select(o => MapToSummaryDto(o));
        }

        public async Task<IEnumerable<OrderSummaryDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _unitOfWork.Orders.GetByDateRangeAsync(startDate, endDate);
            return orders.Select(o => MapToSummaryDto(o));
        }

        public async Task<bool> UpdateStatusAsync(int orderId, short status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;

            await _unitOfWork.Orders.UpdateStatusAsync(orderId, status);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;

            if (order.Status == 3)
                throw new InvalidOperationException(
                    "Cannot cancel an order that has already been delivered.");

            if (order.Status == 4)
                throw new InvalidOperationException(
                    "This order is already cancelled.");

            return await UpdateStatusAsync(orderId, 4); // 4 = Cancelled
        }

        private static string GetStatusLabel(short status) => status switch
        {
            0 => "Pending",
            1 => "Processing",
            2 => "Shipped",
            3 => "Delivered",
            4 => "Cancelled",
            _ => "Unknown"
        };
    }
}
