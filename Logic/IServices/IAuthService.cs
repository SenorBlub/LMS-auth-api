using Logic.Models;

namespace Logic.IServices;

public interface IAuthService
{
	public Task<bool> Authorize(AuthRequest request);
	public Task<(bool, Guid)> EmailAuthorize(EmailAuthRequest request);
	public Task<(bool, Guid)> Register(RegisterRequest request);
}