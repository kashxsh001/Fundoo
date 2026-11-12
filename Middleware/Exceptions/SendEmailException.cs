using System;
using System.Collections.Generic;
using System.Text;

namespace Middleware.Exceptions
{
    public class SendEmailException: Exception
    {
        public SendEmailException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
