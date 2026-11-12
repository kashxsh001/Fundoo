using MediatR;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.User.Command
{
    public record ResetPasswordCommand(ResetPasswordModel Model) : IRequest<bool>;
}
