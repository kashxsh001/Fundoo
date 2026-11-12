using BuisnessLayer.Interface;
using BuisnessLayer.User.Command;
using MediatR;
using Middleware.Services.Hashing;
using Middleware.Services.JWT;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.User.Handlers
{
    public class ResetPasswordHandler: IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IRegisterUserBL _userBL;
        private readonly IJwtTokenService _jwtService;

        public ResetPasswordHandler(
            IRegisterUserBL userBL,
            IJwtTokenService jwtService)
        {
            _userBL = userBL;
            _jwtService = jwtService;
        }

        public async Task<bool> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            ResetPasswordModel model = request.Model;

            if (model == null ||
                string.IsNullOrWhiteSpace(model.token) ||
                string.IsNullOrWhiteSpace(model.newPassword) ||
                model.newPassword != model.confirmPassword)
            {
                throw new ArgumentException("Invalid token or password details.");
            }

            // Validate token and retrieve the associated email.
            string? email =await _jwtService.ValidatePasswordResetTokenAsync(model.token);

            if (string.IsNullOrWhiteSpace(email))return false;

            // Hash in the Handler; repository only updates the database.
            string passwordHash = PasswordHash.Hash(model.newPassword);

            return await _userBL.UpdatePasswordHashAsync(email,passwordHash);
        }
    }
}
