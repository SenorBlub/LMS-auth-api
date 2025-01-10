using System;

namespace Logic.IServices
{
	public interface ITokenService
	{
		string GenerateToken(Guid userId, string username);
		string GenerateRefreshToken(Guid userId, string username);
		bool ValidateToken(string token);
	}
}