using System;

namespace Logic.IServices
{
	public interface ITokenService
	{
		string GenerateToken(Guid userId);
		string GenerateRefreshToken(Guid userId);
		bool ValidateToken(string token);
	}
}