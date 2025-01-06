namespace Logic.Configuration;

public class JwtClaims
{
	public string Sub { get; set; } 
	public string Jti { get; set; } 
	public string Name { get; set; }
	public long Iat { get; set; }
	public long Exp { get; set; }

	public void SetTimestamps(int expirationMinutes)
	{
		var now = DateTimeOffset.UtcNow;
		Iat = now.ToUnixTimeSeconds();
		Exp = now.AddMinutes(expirationMinutes).ToUnixTimeSeconds();
	}
}