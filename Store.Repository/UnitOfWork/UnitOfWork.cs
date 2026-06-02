using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces;
using Store.Repository.Data;
using Store.Repository.Repositories;

namespace Store.Repository.UnitOfWork
{
    // ==================================================
    // Unit of Work
    // Wraps ALL repositories into one object
    // One SaveChangesAsync() commits everything at once
    // ==================================================
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // Lazy initialization — only created when first accessed
        private ICustomerRepository? _customers;
        private IProductRepository? _products;
        private IProductCategoryRepository? _productCategories;
        private IOrderRepository? _orders;
        private IPaymentRepository? _payments;
        private IShippingRepository? _shippings;
        private IReviewRepository? _reviews;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public ICustomerRepository Customers
            => _customers ??= new CustomerRepository(_context);

        public IProductRepository Products
            => _products ??= new ProductRepository(_context);

        public IProductCategoryRepository ProductCategories
            => _productCategories ??= new ProductCategoryRepository(_context);

        public IOrderRepository Orders
            => _orders ??= new OrderRepository(_context);

        public IPaymentRepository Payments
            => _payments ??= new PaymentRepository(_context);

        public IShippingRepository Shippings
            => _shippings ??= new ShippingRepository(_context);

        public IReviewRepository Reviews
            => _reviews ??= new ReviewRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
