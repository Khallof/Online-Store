using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Store.Core.Entities;
using Store.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Store.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ==================================================
        // Generate short-lived Access Token (30 minutes)
        // ==================================================
        public string GenerateAccessToken(Customer customer)
        {
            var key = _configuration["JwtSettings:Key"]!;
            var issuer = _configuration["JwtSettings:Issuer"]!;
            var audience = _configuration["JwtSettings:Audience"]!;
            var duration = int.Parse(_configuration["JwtSettings:DurationInMinutes"] ?? "30");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // ✅ Short claim names — JWT standard
            var claims = new List<Claim>
            {
                new Claim("sub",   customer.CustomerID.ToString()),
                new Claim("email", customer.Email),
                new Claim("name",  customer.Username),
                new Claim("role",  customer.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ==================================================
        // Generate long-lived Refresh Token (random string)
        // Stored in database — not a JWT
        // ==================================================
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
