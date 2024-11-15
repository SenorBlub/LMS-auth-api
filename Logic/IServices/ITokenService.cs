﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IServices
{
	public interface ITokenService
	{
		string GenerateToken(Guid userId);
		string GenerateRefreshToken(Guid userId);
	}
}
