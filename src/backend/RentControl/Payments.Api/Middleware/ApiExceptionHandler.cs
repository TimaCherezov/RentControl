using Microsoft.AspNetCore.Diagnostics;
using Payments.Api.Exceptions;

namespace Payments.Api.Middleware;

public class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is MassTransit.RequestException
            {
                InnerException: not null
            } requestException)
        {
            exception = requestException.InnerException;
        }

        if (exception is not ApiException apiException)
        {
            logger.LogError(exception, "Unhandled exception");
            return false;
        }

        logger.LogWarning(
            apiException,
            "Handled API error: {Message}",
            apiException.Message);

        await WriteErrorsAsync(
            httpContext,
            apiException.StatusCode,
            [
                new
                {
                    Field = string.Empty,
                    Error = apiException.Message
                }
            ],
            cancellationToken);

        return true;
    }

    private static async Task WriteErrorsAsync(
        HttpContext context,
        int statusCode,
        IEnumerable<object> errors,
        CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            errors,
            cancellationToken);
    }
}
