using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RentControl.IntegrationTests.Infrastructure;
using RentControl.IntegrationTests.Payments.Infrastructure;

namespace RentControl.IntegrationTests.Payments;

[Collection(PaymentsApiCollection.Name)]
public class PaymentsApiSmokeTests(PaymentsApiFixture fixture)
{
    [Fact]
    public async Task Alive_ReturnsHealthy()
    {
        var response = await fixture.Client.GetAsync("/alive");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ProtectedEndpoint_WithTokenFromTestKey_IsAccepted()
    {
        using var request = CreatePayInvoiceRequest(fixture.CreateAccessToken(Guid.NewGuid()));

        var response = await fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_IsRejected()
    {
        using var request = CreatePayInvoiceRequest(accessToken: null);

        var response = await fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithTokenSignedByOtherKey_IsRejected()
    {
        // Валидный по форме токен, но подписан чужой парой — JWKS-заглушка его ключ не содержит.
        using var otherKeys = new TestJwtKeys();
        var token = otherKeys.CreateAccessToken(Guid.NewGuid(), fixture.Issuer, fixture.Audience);

        using var request = CreatePayInvoiceRequest(token);

        var response = await fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static HttpRequestMessage CreatePayInvoiceRequest(string? accessToken)
    {
        var bookingId = Guid.NewGuid();
        var request = new HttpRequestMessage(
            HttpMethod.Post, $"api/invoices/by-booking/{bookingId}/pay");

        if (accessToken is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return request;
    }
}
