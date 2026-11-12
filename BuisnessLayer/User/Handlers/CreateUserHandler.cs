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
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserResponseModel?>
    {
        private readonly IRegisterUserBL _userBL;
        private readonly IJwtTokenService _jwt;
        public CreateUserHandler(IRegisterUserBL userBL,IJwtTokenService jwt)
        {
            _userBL = userBL;
            _jwt = jwt;
        }
        public async Task<UserResponseModel?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            RegisterationModel model = request.Model;

            if (string.IsNullOrWhiteSpace(model.fname) ||
           string.IsNullOrWhiteSpace(model.email) ||
           string.IsNullOrWhiteSpace(model.password) ||
           string.IsNullOrWhiteSpace(model.contactno))
            {
                throw new ArgumentException("First name, email, password and phone number are required.");
            }
            string email = model.email.Trim().ToLowerInvariant();
            var existingUser =await _userBL.GetUserByEmailAsync(email);
            if(existingUser != null)
            {
                throw new ArgumentException("A user with this email already exist");
            }
            var userEntity = new UserEntity
            {
                FirstName = model.fname.Trim(),
                LastName = model.lname?.Trim() ?? string.Empty,
                Email = email,
                Password = PasswordHash.Hash(model.password),
                PhoneNumber = model.contactno.Trim()
            };
            UserEntity? savedUser =await _userBL.RegisterUserAsync(userEntity);

            if (savedUser == null)return null;

            // Generate an access token after successful registration.
            string token = _jwt.GenerateAccessToken(savedUser.User_ID,savedUser.Email);

            // Convert saved entity into a safe response model.
            return new UserResponseModel
            {
                userId = savedUser.User_ID,
                fname = savedUser.FirstName,
                lname = savedUser.LastName,
                email = savedUser.Email,
                contactno = savedUser.PhoneNumber,
                token = token
            };
        }
    }

}
