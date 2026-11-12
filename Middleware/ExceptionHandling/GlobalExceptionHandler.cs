using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Middleware.Exceptions;


namespace Middleware.ExceptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception while processing {HttpMethod} {RequestPath}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);

            var (status, title) = exception switch
            {
                ArgumentException or FormatException => (StatusCodes.Status400BadRequest, "Invalid request."),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized."),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found."),
                SendEmailException => (StatusCodes.Status502BadGateway, "Could not send email."),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            var errorResponse = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = "The server could not complete the request.",
                Instance = httpContext.Request.Path
            };
            errorResponse.Extensions["traceId"] = traceId;

            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = "application/problem+json";

            await JsonSerializer.SerializeAsync(
                httpContext.Response.Body,
                errorResponse,
                cancellationToken: cancellationToken);

            return true;
        }
    }
 }

