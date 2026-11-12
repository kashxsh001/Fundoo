using BuisnessLayer.Interface;
using BuisnessLayer.User.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Middleware.Exceptions;
using Middleware.Services.Email;
using Middleware.Services.JWT;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.User.Handlers
{
    public class ForgotPasswordHandler: IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IRegisterUserBL _userBL;
        private readonly IJwtTokenService _jwtService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(
            IRegisterUserBL userBL,
            IJwtTokenService jwtService,
            IEmailSender emailSender,
            ILogger<ForgotPasswordHandler> logger)
        {
            _userBL = userBL;
            _jwtService = jwtService;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request,CancellationToken cancellationToken)
        {
            string email = request.Model.Email.Trim().ToLowerInvariant();

            int? userId = await _userBL.GetUserIdByEmailAsync(email);

            // Don't reveal whether the email exists.
            if (userId == null)return true;

            string token = _jwtService.GeneratePasswordResetToken(userId.Value,email);

            var emailRequest = new EmailRequestModel
            {
                to = email,
                subject = "Fundoo - Reset your password",
                body =
                    "You requested a password reset.\n\n" +
                    "Use this token in the reset-password API:\n\n" +
                    token +
                    "\n\nThis token is valid for 15 minutes."
            };

            try
            {
                await _emailSender.SendEmailAsync(
                    emailRequest,
                    cancellationToken);
            }
            catch (SendEmailException ex)
            {
                _logger.LogError(
                    ex,
                    "Password reset email could not be sent.");
            }

            return true;
        }
    }
}
