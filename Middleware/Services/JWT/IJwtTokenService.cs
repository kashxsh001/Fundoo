using System;
using System.Collections.Generic;
using System.Text;

namespace Middleware.Services.JWT
{
    public interface IJwtTokenService
    {

        string GenerateAccessToken(int userId, string email);
        string GeneratePasswordResetToken(int userId, string email);
        Task<string?> ValidatePasswordResetTokenAsync(string token);
    }
}
