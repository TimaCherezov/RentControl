using System.Text.Json;
using FluentValidation;
using Main.Api.Exceptions;

namespace Main.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized: {Message}", ex.Message);
            await WriteErrorAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Not found: {Message}", ex.Message);
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Conflict: {Message}", ex.Message);
            await WriteErrorAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
            await WriteValidationErrorsAsync(context, ex);
        }
        catch (Exception ex)
        {
            var validationException = FindValidationException(ex);

            if (validationException is not null)
            {
                logger.LogWarning(validationException, "Validation failed (wrapped): {Message}", validationException.Message);
                await WriteValidationErrorsAsync(context, validationException);
                return;
            }

            logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "An internal server error occurred.");
        }
    }

    private static ValidationException? FindValidationException(Exception? exception)
    {
        if (exception is null)
            return null;

        if (exception is ValidationException validationException)
            return validationException;

        if (exception is AggregateException aggregateException)
        {
            foreach (var innerException in aggregateException.InnerExceptions)
            {
                var result = FindValidationException(innerException);

                if (result is not null)
                    return result;
            }
        }

        return FindValidationException(exception.InnerException);
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }

    private static async Task WriteValidationErrorsAsync(HttpContext context, ValidationException ex)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        var problem = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section.15.5.1",
            title = "Validation failed",
            status = StatusCodes.Status400BadRequest,
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
