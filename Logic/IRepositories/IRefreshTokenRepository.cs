using Logic.Models;

namespace Logic.IRepositories;

public interface IRefreshTokenRepository
{
	Task SaveRefreshTokenAsync(RefreshToken refreshToken);
	Task<RefreshToken?> GetRefreshTokenAsync(string token);
	Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
	Task RevokeRefreshTokenAsync(string token);
	Task<bool> IsTokenValidAsync(string token);
}