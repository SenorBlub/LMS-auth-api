namespace Logic.Models;

public class ClaimModel
{
	public Guid Sub { get; set; }
	public Guid Jti { get; set; }
	public DateTime Exp { get; set; }
}