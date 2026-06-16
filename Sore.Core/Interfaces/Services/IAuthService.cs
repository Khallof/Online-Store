using Store.Core.DTOs.Auth;

namespace Store.Core.Interfaces.Services
{
    // ==================================================
    // Auth Service Interface
    // ==================================================
    public interface IAuthService
    {
        // Register a new customer
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        // Login an existing customer
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);

        // Refresh access token using refresh token
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);

        // Revoke a refresh token (logout)
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
}
