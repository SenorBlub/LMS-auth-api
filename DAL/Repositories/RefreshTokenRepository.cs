using DAL.Contexts;
using Logic.IRepositories;
using Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
	private readonly AuthDbContext _context;

	public RefreshTokenRepository(AuthDbContext context)
	{
		_context = context;
	}

	public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
	{
		_context.RefreshTokens.Add(refreshToken);
		await _context.SaveChangesAsync();
	}

	public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
	{
		return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
	}

	public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
	{
		_context.RefreshTokens.Update(refreshToken);
		await _context.SaveChangesAsync();
	}

	public async Task RevokeRefreshTokenAsync(string token)
	{
		var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
		if (refreshToken != null)
		{
			refreshToken.Revoked = true;
			_context.RefreshTokens.Update(refreshToken);
			await _context.SaveChangesAsync();
		}
	}

	public async Task<bool> IsTokenValidAsync(string token)
	{
		var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
		return refreshToken != null && !refreshToken.Revoked && refreshToken.ExpiresAt > DateTime.UtcNow;
	}
}