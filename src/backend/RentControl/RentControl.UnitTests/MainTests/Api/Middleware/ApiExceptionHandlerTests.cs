using FluentValidation;
using FluentValidation.Results;
using Main.Api.Exceptions;
using Main.Api.Middleware;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace RentControl.UnitTests.MainTests.Api.Middleware;

public class ApiExceptionHandlerTests
{
    private readonly ApiExceptionHandler _handler = new(Mock.Of<ILogger<ApiExceptionHandler>>());

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
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }
}
