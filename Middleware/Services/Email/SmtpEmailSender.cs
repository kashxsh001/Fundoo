using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Middleware.Exceptions;
using System.Text;

namespace Middleware.Services.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailRequestModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                string host = _config["Smtp:Host"]!;
                int port = int.Parse(_config["Smtp:Port"]!);
                string username = _config["Smtp:Username"]!;
                string password = _config["Smtp:Password"]!;
                string from = _config["Smtp:From"] ?? username;
                using var message = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = model.subject,
                    Body = model.body,
                    IsBodyHtml = false
                };
                message.To.Add(model.to);

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(username, password)
                };

                await client.SendMailAsync(message, cancellationToken);
                _logger.LogInformation("Email sent to {To}", model.to);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is SmtpException or FormatException or ArgumentException or InvalidOperationException)
            {
                _logger.LogError(ex, "Failed to send email to {To}", model.to);
                throw new SendEmailException("Failed to send email.", ex);
            }
        }
    }
}
