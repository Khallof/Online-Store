using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Repository.Data;

namespace Store.Repository.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                                 .Include(r => r.Customer)
                                 .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByCustomerAsync(int customerId)
        {
            return await _context.RefreshTokens
                                 .Where(r => r.CustomerID == customerId
                                          && r.RevokedAt == null
                                          && r.ExpiresAt > DateTime.UtcNow)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<RefreshToken>> GetAllByCustomerAsync(int customerId)
        {
            return await _context.RefreshTokens
                                 .Where(r => r.CustomerID == customerId)
                                 .OrderByDescending(r => r.CreatedAt)
                                 .ToListAsync();
        }
    }
}
