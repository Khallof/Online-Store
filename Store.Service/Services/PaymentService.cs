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
                // TransactionDate set by DB default
            };
        }

        // ==================================================
        // IGenericService Implementation
        // ==================================================
        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();
            return payments.Select(p => MapToDto(p));
        }

        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null) return null;
            return MapToDto(payment);
        }

        public async Task<PaymentDto> CreateAsync(PaymentCreateDto createDto)
        {
            // Check if order already has a payment
            var alreadyPaid = await _unitOfWork.Payments.OrderHasPaymentAsync(createDto.OrderID);
            if (alreadyPaid)
                throw new InvalidOperationException($"Order {createDto.OrderID} already has a payment.");

            var payment = MapToEntity(createDto);
            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(payment);
        }

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

        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null) return false;

            _unitOfWork.Payments.Delete(payment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // IPaymentService Specific Methods
        // ==================================================
        public async Task<PaymentDto?> GetByOrderAsync(int orderId)
        {
            var payment = await _unitOfWork.Payments.GetByOrderAsync(orderId);
            if (payment == null) return null;
            return MapToDto(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetByMethodAsync(string paymentMethod)
        {
            var payments = await _unitOfWork.Payments.GetByMethodAsync(paymentMethod);
            return payments.Select(p => MapToDto(p));
        }

        public async Task<IEnumerable<PaymentDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _unitOfWork.Payments.GetByDateRangeAsync(startDate, endDate);
            return payments.Select(p => MapToDto(p));
        }

        public async Task<bool> OrderHasPaymentAsync(int orderId)
        {
            return await _unitOfWork.Payments.OrderHasPaymentAsync(orderId);
        }
    }
}
