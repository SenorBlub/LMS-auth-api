namespace Logic.Models;

public class AuthResponse
{
	public Guid UserId { get; set; }
	public string Token { get; set; }
	public string RefreshToken { get; set; }
}
