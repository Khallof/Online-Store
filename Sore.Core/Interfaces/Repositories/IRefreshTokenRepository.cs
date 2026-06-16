using Store.Core.Entities;

namespace Store.Core.Interfaces.Repositories
{
    // ==================================================
    // Refresh Token Repository Interface
    // ==================================================
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        // Get refresh token by token value
        Task<RefreshToken?> GetByTokenAsync(string token);

        // Get all active tokens for a customer
        Task<IEnumerable<RefreshToken>> GetActiveTokensByCustomerAsync(int customerId);

        // Get all tokens for a customer
        Task<IEnumerable<RefreshToken>> GetAllByCustomerAsync(int customerId);
    }
}
