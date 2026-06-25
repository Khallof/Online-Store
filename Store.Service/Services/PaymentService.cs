using Store.Core.DTOs.Payment;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================
        // Mapping Methods
        // ==================================================
        private PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                PaymentID = payment.PaymentID,
                OrderID = payment.OrderID,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                TransactionDate = payment.TransactionDate
            };
        }

        private Payment MapToEntity(PaymentCreateDto dto)
        {
            return new Payment
            {
                OrderID = dto.OrderID,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod
            };
        }

        // ==================================================
        // GetAllAsync
        // ==================================================
        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();
            return payments.Select(p => MapToDto(p));
        }

        // ==================================================
        // GetByIdAsync
        // ==================================================
        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null) return null;
            return MapToDto(payment);
        }

        // ==================================================
        // CreateAsync
        //     Business Validations:
        // 1 — Can't pay for already paid order
        // 2 — Can't pay for a cancelled order
        // 3 — Payment amount must match order total
        // ==================================================
        public async Task<PaymentDto> CreateAsync(PaymentCreateDto createDto)
        {
            //  Validation 1 — Already paid?
            var alreadyPaid = await _unitOfWork.Payments
                                               .OrderHasPaymentAsync(createDto.OrderID);
            if (alreadyPaid)
                throw new InvalidOperationException(
                    $"Order {createDto.OrderID} already has a payment.");

            //  Validation 2 — Can't pay for cancelled order
            var order = await _unitOfWork.Orders.GetByIdAsync(createDto.OrderID);
            if (order == null)
                throw new InvalidOperationException(
                    $"Order {createDto.OrderID} does not exist.");

            if (order.Status == 4)
                throw new InvalidOperationException(
                    "Cannot pay for a cancelled order.");

            //  Validation 3 — Payment amount must match order total
            if (createDto.Amount != order.TotalAmount)
                throw new InvalidOperationException(
                    $"Payment amount ({createDto.Amount:C}) does not match " +
                    $"order total ({order.TotalAmount:C}). " +
                    $"Please pay the exact amount.");

            var payment = MapToEntity(createDto);
            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(payment);
        }

        // ==================================================
        // UpdateAsync
        // ==================================================
        public async Task<PaymentDto?> UpdateAsync(int id, PaymentCreateDto updateDto)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null) return null;

            payment.Amount = updateDto.Amount;
            payment.PaymentMethod = updateDto.PaymentMethod;

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(payment);
        }

        // ==================================================
        // DeleteAsync
        // ==================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null) return false;

            _unitOfWork.Payments.Delete(payment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // GetByOrderAsync
        // ==================================================
        public async Task<PaymentDto?> GetByOrderAsync(int orderId)
        {
            var payment = await _unitOfWork.Payments.GetByOrderAsync(orderId);
            if (payment == null) return null;
            return MapToDto(payment);
        }

        // ==================================================
        // GetByMethodAsync
        // ==================================================
        public async Task<IEnumerable<PaymentDto>> GetByMethodAsync(string paymentMethod)
        {
            var payments = await _unitOfWork.Payments.GetByMethodAsync(paymentMethod);
            return payments.Select(p => MapToDto(p));
        }

        // ==================================================
        // GetByDateRangeAsync
        // ==================================================
        public async Task<IEnumerable<PaymentDto>> GetByDateRangeAsync(
            DateTime startDate, DateTime endDate)
        {
            var payments = await _unitOfWork.Payments
                                            .GetByDateRangeAsync(startDate, endDate);
            return payments.Select(p => MapToDto(p));
        }

        // ==================================================
        // OrderHasPaymentAsync
        // ==================================================
        public async Task<bool> OrderHasPaymentAsync(int orderId)
        {
            return await _unitOfWork.Payments.OrderHasPaymentAsync(orderId);
        }
    }
}