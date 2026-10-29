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
    public async Task WeatherForecast_WithTokenFromTestKey_IsAccepted()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/weatherforecast");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", fixture.CreateAccessToken(Guid.NewGuid()));

        var response = await fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task WeatherForecast_WithoutToken_IsRejected()
    {
        var response = await fixture.Client.GetAsync("/weatherforecast");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WeatherForecast_WithTokenSignedByOtherKey_IsRejected()
    {
        // Валидный по форме токен, но подписан чужой парой — JWKS-заглушка его ключ не содержит.
        using var otherKeys = new TestJwtKeys();
        var token = otherKeys.CreateAccessToken(Guid.NewGuid(), fixture.Issuer, fixture.Audience);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/weatherforecast");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
