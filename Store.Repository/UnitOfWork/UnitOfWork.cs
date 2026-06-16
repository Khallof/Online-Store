using Store.Core.Interfaces;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;
using Store.Repository.Repositories;

namespace Store.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ICustomerRepository Customers { get; }
        public IProductRepository Products { get; }
        public IProductCategoryRepository ProductCategories { get; }
        public IOrderRepository Orders { get; }
        public IPaymentRepository Payments { get; }
        public IShippingRepository Shippings { get; }
        public IReviewRepository Reviews { get; }

        // ✅ New
        public IRefreshTokenRepository RefreshTokens { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Customers = new CustomerRepository(_context);
            Products = new ProductRepository(_context);
            ProductCategories = new ProductCategoryRepository(_context);
            Orders = new OrderRepository(_context);
            Payments = new PaymentRepository(_context);
            Shippings = new ShippingRepository(_context);
            Reviews = new ReviewRepository(_context);

            // ✅ New
            RefreshTokens = new RefreshTokenRepository(_context);
        }

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
