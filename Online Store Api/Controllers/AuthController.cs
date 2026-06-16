using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Store.API.Helpers;
using Store.Core.DTOs.Auth;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ==================================================
        // POST api/auth/register
        // ==================================================
        [HttpPost("register")]
        [EnableRateLimiting("AuthPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registration successful"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.Fail(ex.Message));
            }
        }

        // ==================================================
        // POST api/auth/login
        // ==================================================
        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return Unauthorized(ApiResponse<AuthResponseDto>.Fail("Invalid email or password"));

            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful"));
        }

        // ==================================================
        // POST api/auth/refresh
        // Get new access token using refresh token
        // ==================================================
        [HttpPost("refresh")]
        [EnableRateLimiting("WritePolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            if (result == null)
                return Unauthorized(ApiResponse<AuthResponseDto>.Fail("Invalid or expired refresh token"));

            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Token refreshed successfully"));
        }

        // ==================================================
        // POST api/auth/logout
        // Revoke refresh token
        // ==================================================
        [HttpPost("logout")]
        [Authorize]
        [EnableRateLimiting("WritePolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> Logout(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RevokeTokenAsync(refreshTokenDto.RefreshToken);
            if (!result)
                return BadRequest(ApiResponse<bool>.Fail("Invalid or already revoked token"));

            return Ok(ApiResponse<bool>.Ok(true, "Logged out successfully"));
        }

   
    }
}
