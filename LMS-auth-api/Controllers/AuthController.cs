using Logic.IRepositories;
using Logic.IServices;
using Logic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS_auth_api.Controllers
{
	[Route("auth/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly ITokenService _tokenService;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IAuthService _authService;

		public AuthController(ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IAuthService authservice)
		{
			_tokenService = tokenService;
			_refreshTokenRepository = refreshTokenRepository;
			_authService = authservice;
		}

		[HttpPost("authorize")]
		public async Task<IActionResult> Login([FromBody] AuthRequest request)
		{
			if (!_authService.Authorize(request).Result)
			{
				return BadRequest("Login failed, incorrect credentials");
			}
			else
			{
				//check password correct via user api before token handout
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
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequest request)
		{
			var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken);
			if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow || refreshToken.Revoked)
			{
				return Unauthorized("Invalid refresh token.");
			}

			var newAccessToken = _tokenService.GenerateToken(refreshToken.UserId);
			var newRefreshToken = refreshToken;
			newRefreshToken.Token = _tokenService.GenerateRefreshToken(refreshToken.UserId);
			newRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(1);
			newRefreshToken.Id = Guid.NewGuid();
			await _refreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken);
			await _refreshTokenRepository.RevokeRefreshTokenAsync(request.RefreshToken);
			return Ok(new AuthResponse { Token = newAccessToken, RefreshToken = newRefreshToken.Token });
		}
	}
}
