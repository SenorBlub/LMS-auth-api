﻿using Logic.Models;

namespace Logic.IServices;

public interface IAuthService
{
	public Task<bool> Authorize(AuthRequest request);
}