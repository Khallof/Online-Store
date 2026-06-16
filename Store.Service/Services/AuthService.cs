using Store.Core.DTOs.Auth;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        // Refresh token duration in days
        private const int RefreshTokenDurationDays = 7;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        // ==================================================
        // Register
        // ==================================================
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // 1 — Check duplicates
            if (await _unitOfWork.Customers.EmailExistsAsync(registerDto.Email))
                throw new InvalidOperationException("Email already exists");

            if (await _unitOfWork.Customers.UsernameExistsAsync(registerDto.Username))
                throw new InvalidOperationException("Username already exists");

            // 2 — Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // 3 — Create customer
            var customer = new Customer
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Username = registerDto.Username,
                Password = hashedPassword,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                Role = "Customer"
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            // 4 — Generate tokens
            return await GenerateAuthResponse(customer);
        }

        // ==================================================
        // Login
        // ==================================================
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // 1 — Find customer
            var customer = await _unitOfWork.Customers.GetByEmailAsync(loginDto.Email);
            if (customer == null) return null;

            // 2 — Verify password
            var isValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, customer.Password);
            if (!isValid) return null;

            // 3 — Revoke all old refresh tokens for this customer
            await RevokeAllCustomerTokensAsync(customer.CustomerID);

            // 4 — Generate new tokens
            return await GenerateAuthResponse(customer);
        }

        // ==================================================
        // Refresh Token
        // ==================================================
        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            // 1 — Find the refresh token in database
            var storedToken = await _unitOfWork.RefreshTokens
                                               .GetByTokenAsync(refreshToken);

            // 2 — Validate token
            if (storedToken == null || !storedToken.IsActive)
                return null;

            // 3 — Get the customer
            var customer = await _unitOfWork.Customers
                                            .GetByIdAsync(storedToken.CustomerID);
            if (customer == null) return null;

            // 4 — Revoke the used refresh token
            storedToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(storedToken);

            // 5 — Generate new tokens
            return await GenerateAuthResponse(customer);
        }

        // ==================================================
        // Revoke Token (Logout)
        // ==================================================
        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var storedToken = await _unitOfWork.RefreshTokens
                                               .GetByTokenAsync(refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return false;

            storedToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(storedToken);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ==================================================
        // Private helper — generates both tokens
        // ==================================================
        private async Task<AuthResponseDto> GenerateAuthResponse(Customer customer)
        {
            // Generate access token
            var accessToken = _tokenService.GenerateAccessToken(customer);

            // Generate refresh token
            var refreshTokenValue = _tokenService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshTokenValue,
                CustomerID = customer.CustomerID,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDurationDays)
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                Email = customer.Email,
                Username = customer.Username,
                Role = customer.Role,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(30),
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDurationDays)
            };
        }

        // ==================================================
        // Private helper — revokes all tokens for a customer
        // ==================================================
        private async Task RevokeAllCustomerTokensAsync(int customerId)
        {
            var tokens = await _unitOfWork.RefreshTokens
                                          .GetActiveTokensByCustomerAsync(customerId);

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                _unitOfWork.RefreshTokens.Update(token);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
