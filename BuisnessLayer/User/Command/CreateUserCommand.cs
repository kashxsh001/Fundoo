using MediatR;
using Middleware.Services.Hashing;
using ModelLayer;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.User.Command
{
    public record CreateUserCommand(RegisterationModel Model) : IRequest<UserResponseModel?>;

}
