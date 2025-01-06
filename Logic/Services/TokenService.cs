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

	public TokenService(IOptions<JwtConfig> jwtConfigOptions)
	{
		_jwtConfig = jwtConfigOptions.Value ?? throw new ArgumentNullException(nameof(jwtConfigOptions));
	}

	public string GenerateToken(Guid userId)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddHours(1),
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

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddDays(1),
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
