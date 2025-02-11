﻿namespace Logic.Configuration;

public class JwtClaims
{
	public string Sub { get; set; } 
	public string Jti { get; set; }
	public long Iat { get; set; }
	public long? Nbf { get; set; }
	public long Exp { get; set; }
	public void SetTimestamps(int expirationMinutes)
	{
		var now = DateTimeOffset.UtcNow;
		Iat = now.ToUnixTimeSeconds();
		Nbf = now.ToUnixTimeSeconds();
		Exp = now.AddMinutes(expirationMinutes).ToUnixTimeSeconds();
	}
}