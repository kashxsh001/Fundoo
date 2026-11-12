using ModelLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Middleware.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailRequestModel model, CancellationToken cancellationToken = default);
    }
}
