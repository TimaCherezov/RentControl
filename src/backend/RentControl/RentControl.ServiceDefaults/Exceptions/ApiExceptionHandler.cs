using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RentControl.ServiceDefaults.Exceptions;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    ApiExceptionHandlerBuilder? builder = null) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        exception = builder?.UnwrapException(exception) ?? exception;

        if (exception is ApiException apiException)
        {
            logger.LogWarning(
                apiException,
                "Handled API error: {Message}",
                apiException.Message);

            await WriteErrorsAsync(
                httpContext,
                apiException.StatusCode,
                [new ApiError(string.Empty, apiException.Message)],
                cancellationToken);

            return true;
        }

        if (builder?.MapException(exception) is { } mapped)
        {
            logger.LogWarning(
                exception,
                "Handled mapped error: {Message}",
                exception.Message);

            await WriteErrorsAsync(
                httpContext,
                mapped.StatusCode,
                mapped.Errors,
                cancellationToken);

            return true;
        }

        logger.LogError(exception, "Unhandled exception");
        return false;
    }

    private static async Task WriteErrorsAsync(
        HttpContext context,
        int statusCode,
        IEnumerable<ApiError> errors,
        CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(errors, cancellationToken);
    }
}
