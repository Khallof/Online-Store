using Store.Core.Entities;

namespace Store.Core.Interfaces.Services
{
    // ==================================================
    // Token Service Interface
    // Handles both Access Token and Refresh Token
    // ==================================================
    public interface ITokenService
    {
        
        string GenerateAccessToken(Customer customer);

        
        string GenerateRefreshToken();
    }
}
