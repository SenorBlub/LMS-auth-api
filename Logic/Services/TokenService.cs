using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Logic.Configuration;
using Logic.IServices;
using Logic.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
	private readonly JwtConfig _jwtConfig;

	public TokenService(IOptions<JwtConfig> jwtConfigOptions)
	{
		_jwtConfig = jwtConfigOptions.Value;
	}

	public string GenerateToken(Guid userId)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

		ClaimModel claimModel = new ClaimModel
		{
			Sub = userId,
			Jti = Guid.NewGuid(),
			Exp = DateTime.UtcNow.AddHours(1),
		};

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, claimModel.Sub.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, claimModel.Jti.ToString()),
			new Claim(JwtRegisteredClaimNames.Exp, claimModel.Exp.ToString())
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddHours(1),  
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public string GenerateRefreshToken(Guid userId)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

		ClaimModel claimModel = new ClaimModel
		{
			Sub = userId,
			Jti = Guid.NewGuid(),
			Exp = DateTime.UtcNow.AddHours(1),
		};

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, claimModel.Sub.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, claimModel.Jti.ToString()),
			new Claim(JwtRegisteredClaimNames.Exp, claimModel.Exp.ToString())
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddDays(1),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}