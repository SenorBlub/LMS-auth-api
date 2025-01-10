using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Logic.Configuration;
using Logic.IServices;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
	private readonly JwtConfig _jwtConfig;

	public TokenService(JwtConfig jwtConfig)
	{
		_jwtConfig = jwtConfig;
	}

	public string GenerateToken(Guid userId)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

		var claims = new JwtClaims
		{
			Sub = userId.ToString(),
			Jti = Guid.NewGuid().ToString(),
		};
		claims.SetTimestamps(60); // Token valid for 60 minutes

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, claims.Sub),
				new Claim(JwtRegisteredClaimNames.Jti, claims.Jti),
				new Claim(JwtRegisteredClaimNames.Iat, claims.Iat.ToString(), ClaimValueTypes.Integer64)
			}),
			Expires = DateTimeOffset.FromUnixTimeSeconds(claims.Exp).UtcDateTime,
			Issuer = _jwtConfig.Issuer,
			Audience = _jwtConfig.Audience,
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public string GenerateRefreshToken(Guid userId)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

		var claims = new JwtClaims
		{
			Sub = userId.ToString(),
			Jti = Guid.NewGuid().ToString()
		};
		claims.SetTimestamps(1440); // Token valid for 1 day

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, claims.Sub),
				new Claim(JwtRegisteredClaimNames.Jti, claims.Jti),
				new Claim(JwtRegisteredClaimNames.Iat, claims.Iat.ToString(), ClaimValueTypes.Integer64)
			}),
			Expires = DateTimeOffset.FromUnixTimeSeconds(claims.Exp).UtcDateTime,
			Issuer = _jwtConfig.Issuer,
			Audience = _jwtConfig.Audience,
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public bool ValidateToken(string token)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

		try
		{
			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = true,
				ValidIssuer = _jwtConfig.Issuer,
				ValidateAudience = true,
				ValidAudience = _jwtConfig.Audience,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			}, out _);

			return true;
		}
		catch
		{
			return false;
		}
	}
}
