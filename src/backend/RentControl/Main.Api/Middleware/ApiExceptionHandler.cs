using FluentValidation;
using Main.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Main.Api.Middleware;

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

        switch (exception)
        {
            case ApiException apiException:
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

            case ValidationException validationException:
                logger.LogWarning(
                    validationException,
                    "Handled validation error: {Message}",
                    validationException.Message);

                var errors = validationException.Errors
                    .Select(error => new
                    {
                        Field = error.PropertyName,
                        Error = error.ErrorMessage
                    })
                    .ToArray();

                await WriteErrorsAsync(
                    httpContext,
                    StatusCodes.Status400BadRequest,
                    errors,
                    cancellationToken);

                return true;

            default:
                logger.LogError(exception, "Unhandled exception");
                return false;
        }
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
