using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Payments.Api.Exceptions;
using Payments.Api.Middleware;

namespace RentControl.UnitTests.PaymentsTests.Api.Middleware;

public class ApiExceptionHandlerTests
{
    private readonly ApiExceptionHandler _handler = new(Mock.Of<ILogger<ApiExceptionHandler>>());

    [Fact]
    public async Task TryHandle_WhenObjectNotFound_Writes404WithError()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new ObjectNotFoundException("Счёт не найден"),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Contains("Счёт не найден", await ReadBody(context));
    }

    [Fact]
    public async Task TryHandle_WhenPaymentAlreadyPaid_Writes409WithError()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new PaymentAlreadyPaidException("Счёт уже оплачен"),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(409, context.Response.StatusCode);
        Assert.Contains("Счёт уже оплачен", await ReadBody(context));
    }

    [Fact]
    public async Task TryHandle_WhenRequestExceptionWrapsApiException_UnwrapsToInnerApiException()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new RequestException("outer failure", new ObjectNotFoundException("Счёт по брони не найден")),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Contains("Счёт по брони не найден", await ReadBody(context));
    }

    [Fact]
    public async Task TryHandle_WhenRequestExceptionWithoutInner_ReturnsFalse()
    {
        var context = CreateHttpContext();

        var handled = await _handler.TryHandleAsync(
            context,
            new RequestException("transport failure"),
            CancellationToken.None);

        Assert.False(handled);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandle_WhenUnhandledException_ReturnsFalseAndDoesNotWriteResponse()
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
