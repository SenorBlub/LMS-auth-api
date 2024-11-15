namespace Logic.Models;

public class RefreshToken
{
	public Guid Id { get; set; }
	public string Token { get; set; }
	public Guid UserId { get; set; }
	public DateTime ExpiresAt { get; set; }
	public DateTime CreatedAt { get; set; }
	public bool Revoked { get; set; }
}
