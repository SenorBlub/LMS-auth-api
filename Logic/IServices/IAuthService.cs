using Logic.Models;

namespace Logic.IServices;

public interface IAuthService
{
	public Task<bool> Authorize(AuthRequest request);
	public Task<(bool, Guid)> Authorize(EmailAuthRequest request);
	public Task<bool> Register(RegisterRequest request);
}