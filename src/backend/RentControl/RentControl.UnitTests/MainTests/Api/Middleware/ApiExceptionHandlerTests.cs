using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using RentControl.ServiceDefaults.Exceptions;

namespace RentControl.UnitTests.MainTests.Api.Middleware;

public class ApiExceptionHandlerTests
{
    private readonly ApiExceptionHandler _handler = CreateHandler();

    private static ApiExceptionHandler CreateHandler()
    {
        var builder = new ApiExceptionHandlerBuilder()
            .Unwrap<RequestException>()
            .Map<ValidationException>(ex => new ApiErrorResponse(
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new ApiError(e.PropertyName, e.ErrorMessage)).ToArray()))
            .Map<DbUpdateException>(ex =>
                ex.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.UniqueViolation
                }
                    ? new ApiErrorResponse(
                        StatusCodes.Status409Conflict,
                        [new ApiError(string.Empty, "Resource cannot be modified or deleted because it is referenced or conflicts with existing data.")])
                    : null);

        return new ApiExceptionHandler(Mock.Of<ILogger<ApiExceptionHandler>>(), builder);
    }

    [Fact]
    public async Task TryHandle_WhenApiException_WritesStatusCodeAndError()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new ObjectNotFoundException("Product not found"),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Contains("Product not found", await ReadBody(context));
    }

    [Fact]
    public async Task TryHandle_WhenForbiddenException_Writes403AndError()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new ForbiddenException("Access to this booking is denied."),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(403, context.Response.StatusCode);
        Assert.Contains("Access to this booking is denied.", await ReadBody(context));
    }

    [Fact]
    public async Task TryHandle_WhenRequestExceptionWrapsApiException_UnwrapsInnerException()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new RequestException("outer failure", new ObjectNotFoundException("Booking not found")),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Contains("Booking not found", await ReadBody(context));
    }

    [Fact]
    public async Task TryHandle_WhenValidationException_WritesFieldErrorsWith400()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new ValidationException([new ValidationFailure("Email", "Некорректный email.")]),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(400, context.Response.StatusCode);
        var body = await ReadBody(context);
        Assert.Contains("Email", body);
        Assert.Contains("Некорректный email.", body);
    }

    [Theory]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation)]
    [InlineData(PostgresErrorCodes.UniqueViolation)]
    public async Task TryHandle_WhenDbUpdateExceptionWithConflictSqlState_Writes409(string sqlState)
    {
        var context = CreateHttpContext();
        var pgException = new PostgresException("conflict", "ERROR", "ERROR", sqlState);
        var dbException = new DbUpdateException("save failed", pgException);

        var handled = await _handler.TryHandleAsync(
            context,
            new RequestException("outer", dbException),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(409, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandle_WhenDbUpdateExceptionWithNonConflictSqlState_ReturnsFalse()
    {
        var context = CreateHttpContext();
        var pgException = new PostgresException("not null", "ERROR", "ERROR", PostgresErrorCodes.NotNullViolation);
        var dbException = new DbUpdateException("save failed", pgException);

        var handled = await _handler.TryHandleAsync(
            context,
            dbException,
            CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandle_WhenUnexpectedException_ReturnsFalseAndDoesNotWriteResponse()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new InvalidOperationException("boom"),
            CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(200, context.Response.StatusCode);
        Assert.Equal(string.Empty, await ReadBody(context));
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(
            context.Response.Body,
            System.Text.Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: -1,
            leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}
