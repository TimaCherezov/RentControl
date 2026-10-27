using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RentControl.E2ETests;

[Collection(AppHostCollection.Name)]
public sealed class CrossServiceAuthTests(AppHostFixture fixture)
{
    private const string Issuer = "RentControl";
    private const string Audience = "RentControl";
    private const string ProtectedEndpoint = "weatherforecast";

    [Fact]
    public async Task Payments_AcceptsTokenIssuedByMain()
    {
        var accessToken = await GetMainAccessTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, ProtectedEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await fixture.PaymentsApi.SendAsync(
            request, TestContext.Current.CancellationToken);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Payments_RejectsTokenSignedByForeignKey()
    {
        var forgedToken = JwtTokenCreator.CreateTokenSignedByForeignKey(Issuer, Audience);

        using var request = new HttpRequestMessage(HttpMethod.Get, ProtectedEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", forgedToken);

        using var response = await fixture.PaymentsApi.SendAsync(
            request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<string> GetMainAccessTokenAsync()
    {
        var email = $"e2e-{Guid.NewGuid():N}@test.local";
        var password = "e2e-password-123";

        using var register = await fixture.MainApi.PostAsJsonAsync(
            "api/auth/register",
            new { name = "E2E", email, password });
        Assert.True(register.IsSuccessStatusCode,
            $"Регистрация в Main не удалась: {(int)register.StatusCode}");

        using var login = await fixture.MainApi.PostAsJsonAsync(
            "api/auth/login",
            new { email, password });
        Assert.True(login.IsSuccessStatusCode,
            $"Логин в Main не удался: {(int)login.StatusCode}");

        var body = await login.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = body.GetProperty("accessToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(accessToken), "Main не вернул accessToken");
        return accessToken!;
    }
}