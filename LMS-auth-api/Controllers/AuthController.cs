using Logic.IRepositories;
using Logic.IServices;
using Logic.Models;
using Microsoft.AspNetCore.Mvc;

namespace LMS_auth_api.Controllers
{
	[Route("auth/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ITokenService _tokenService;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IAuthService _authService;

		public AuthController(ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IAuthService authService)
		{
			_tokenService = tokenService;
			_refreshTokenRepository = refreshTokenRepository;
			_authService = authService;
		}

		[HttpPost("authorize")]
		public async Task<IActionResult> Login([FromBody] AuthRequest request)
		{
			if (!await _authService.Authorize(request))
			{
				return BadRequest("Login failed, incorrect credentials.");
			}

			var token = _tokenService.GenerateToken(request.UserId);
			var refreshToken = _tokenService.GenerateRefreshToken(request.UserId);

			await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
			{
				Token = refreshToken,
				UserId = request.UserId,
				ExpiresAt = DateTime.UtcNow.AddDays(1)
			});

			return Ok(new AuthResponse { Token = token, RefreshToken = refreshToken });
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequest request)
		{
			var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken);

			if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow || refreshToken.Revoked)
			{
				return Unauthorized("Invalid or expired refresh token.");
			}

			var newAccessToken = _tokenService.GenerateToken(refreshToken.UserId);
			refreshToken.Token = _tokenService.GenerateRefreshToken(refreshToken.UserId);
			refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(1);
			refreshToken.Id = Guid.NewGuid();

			await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken);
			await _refreshTokenRepository.RevokeRefreshTokenAsync(request.RefreshToken);

			return Ok(new AuthResponse { Token = newAccessToken, RefreshToken = refreshToken.Token });
		}
	}
}
