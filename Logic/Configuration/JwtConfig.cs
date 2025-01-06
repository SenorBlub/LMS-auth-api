namespace Logic.Configuration;

public class JwtConfig
{
	public string Secret { get; set; } 
	public string Issuer { get; set; } 
	public string Audience { get; set; } 
	public string Alg { get; set; } = "HS256"; 
	public string Typ { get; set; } = "JWT"; 
}