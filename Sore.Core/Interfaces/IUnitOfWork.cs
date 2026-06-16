using Store.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // All repositories accessible through UnitOfWork
        ICustomerRepository Customers { get; }
        IProductRepository Products { get; }
        IProductCategoryRepository ProductCategories { get; }
        IOrderRepository Orders { get; }
        IPaymentRepository Payments { get; }
        IShippingRepository Shippings { get; }
        IReviewRepository Reviews { get; }

        IRefreshTokenRepository RefreshTokens { get; }
        // Save all changes to the database in one transaction
        Task<int> SaveChangesAsync();
    }
}
