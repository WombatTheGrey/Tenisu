using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Tenisu.Domain.Exceptions;

namespace Tenisu.WebApi.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError("An exception on route {path} has been handled : {exception}", httpContext.Request.Path, exception);

            var (statusCode, message) = exception switch
            {
                ArgumentNullException => (StatusCodes.Status422UnprocessableEntity, exception.Message),
                ArgumentOutOfRangeException => (StatusCodes.Status422UnprocessableEntity, exception.Message),
                ArgumentException => (StatusCodes.Status422UnprocessableEntity, exception.Message),
                EntityAlreadyExistsException => (StatusCodes.Status409Conflict, exception.Message),
                DomainException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, $"An unexpected error occurred")
            };

            httpContext.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails()
            {
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Status = statusCode,
                Detail = message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
