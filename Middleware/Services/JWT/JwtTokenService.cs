using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Middleware.Services.JWT
{
    public class JwtTokenService : IJwtTokenService
    {
        private const string AccessTokenUse = "access";
        private const string ResetTokenUse = "password_reset";
        private const string TokenUseClaim = "token_use";

        private readonly IConfiguration _configuration;
        private readonly JsonWebTokenHandler _handler = new();

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(int userId, string email)
        {
            return CreateToken(userId, email, AccessTokenUse,
                "Jwt:SigningKey", "Jwt:AccessTokenMinutes", defaultMinutes: 60);
        }

        public string GeneratePasswordResetToken(int userId, string email)
        {
            return CreateToken(userId, email, ResetTokenUse,
                "JwtReset:ResetKey", "Jwt:ResetTokenMinutes", defaultMinutes: 15);
        }

        public async Task<string?> ValidatePasswordResetTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = GetKey("JwtReset:ResetKey"),
                ClockSkew = TimeSpan.Zero
            };

            var result = await _handler.ValidateTokenAsync(token.Trim(), parameters);
            if (!result.IsValid) return null;

            // an access token must never be accepted as a reset token
            if (result.ClaimsIdentity.FindFirst(TokenUseClaim)?.Value != ResetTokenUse) return null;

            return result.ClaimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
        }

        private string CreateToken(int userId, string email, string tokenUse,
                                   string keyConfig, string minutesConfig, int defaultMinutes)
        {
            int minutes = int.TryParse(_configuration[minutesConfig], out var m) ? m : defaultMinutes;

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(minutes),
                SigningCredentials = new SigningCredentials(GetKey(keyConfig), SecurityAlgorithms.HmacSha256),
                Claims = new Dictionary<string, object>
                {
                    ["user_id"] = userId.ToString(),
                    [ClaimTypes.NameIdentifier] = userId.ToString(),
                    [ClaimTypes.Email] = email,
                    [TokenUseClaim] = tokenUse
                }
            };
            return _handler.CreateToken(descriptor);
        }

        private SymmetricSecurityKey GetKey(string keyConfig)
        {
            string key = _configuration[keyConfig]
                ?? throw new InvalidOperationException($"Missing configuration value '{keyConfig}'.");
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
    }
}
