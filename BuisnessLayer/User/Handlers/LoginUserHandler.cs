using BuisnessLayer.Interface;
using BuisnessLayer.User.Command;
using MediatR;
using Middleware.Services.Hashing;
using Middleware.Services.JWT;
using ModelLayer;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.User.Handlers
{
    public class LoginUserHandler: IRequestHandler<LoginUserCommand, UserResponseModel?>
    {
        private readonly IRegisterUserBL _userBL;
        private readonly IJwtTokenService _jwtService;

        public LoginUserHandler(IRegisterUserBL userBL,IJwtTokenService jwtService)
        {
            _userBL = userBL;
            _jwtService = jwtService;
        }

        public async Task<UserResponseModel?> Handle(LoginUserCommand request,CancellationToken cancellationToken)
        {
            LoginModel model = request.Model;

            if (model == null ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentException("Email and password are required.");
            }

            string email = model.Email.Trim().ToLowerInvariant();

            // Retrieve the user through BL -> RL.
            UserEntity? user = await _userBL.GetUserByEmailAsync(email);

            // Return the same result for an unknown email or wrong password.
            if (user == null ||!PasswordHash.Verify(model.Password, user.Password))
            {
                return null;
            }

            string token = _jwtService.GenerateAccessToken(user.User_ID,user.Email);

            return new UserResponseModel
            {
                userId = user.User_ID,
                fname = user.FirstName,
                lname = user.LastName,
                email = user.Email,
                contactno = user.PhoneNumber,
                token = token
            };
        }
    }
}
