using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Logic.Configuration;
using Logic.IServices;
using Microsoft.IdentityModel.Tokens;

namespace Logic.Services;

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

		var now = DateTimeOffset.UtcNow;

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Iss, _jwtConfig.Issuer),
			new Claim(JwtRegisteredClaimNames.Aud, _jwtConfig.Audience)
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			IssuedAt = now.UtcDateTime,
			Expires = now.AddMinutes(60).UtcDateTime,
			NotBefore = now.UtcDateTime,
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public string GenerateRefreshToken(Guid userId)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

		var now = DateTimeOffset.UtcNow;

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Iss, _jwtConfig.Issuer),
			new Claim(JwtRegisteredClaimNames.Aud, _jwtConfig.Audience)
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			IssuedAt = now.UtcDateTime,
			Expires = now.AddDays(1).UtcDateTime,
			NotBefore = now.UtcDateTime,
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
			var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = true,
				ValidIssuer = _jwtConfig.Issuer,
				ValidateAudience = true,
				ValidAudience = _jwtConfig.Audience,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

			if (validatedToken is JwtSecurityToken jwtToken)
			{
				if (!jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				{
					throw new SecurityTokenException("Invalid token algorithm.");
				}

				var claims = jwtToken.Claims;
				var sub = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
				var jti = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
				var iat = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat)?.Value;
				var nbf = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Nbf)?.Value;

				if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(iat) || string.IsNullOrEmpty(nbf))
				{
					throw new SecurityTokenException("Missing required claims.");
				}

				if (DateTime.TryParse(nbf, out var notBeforeTime) && DateTime.UtcNow < notBeforeTime)
				{
					throw new SecurityTokenException("Token is not yet valid.");
				}
			}
			else
			{
				throw new SecurityTokenException("Invalid token format.");
			}

			return true; 
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Token validation failed: {ex.Message}");
			return false; 
		}
	}

}