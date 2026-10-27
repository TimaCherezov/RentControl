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
    private const string RefreshCookieName = "refresh_token";
    private const string Password = "e2e-password-123";

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
    public async Task Payments_AcceptsTokenIssuedByMainAfterRefresh()
    {
        var email = await RegisterUserAsync();

        using var login = await fixture.MainApi.PostAsJsonAsync(
            "api/auth/login",
            new { email, password = Password },
            TestContext.Current.CancellationToken);
        Assert.True(login.IsSuccessStatusCode,
            $"Логин в Main не удался: {(int)login.StatusCode}");

        var refreshToken = GetRefreshCookie(login);
        Assert.False(string.IsNullOrWhiteSpace(refreshToken),
            "Main не установил куку refresh_token при логине");

        using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
        refreshRequest.Headers.TryAddWithoutValidation(
            "Cookie", $"{RefreshCookieName}={refreshToken}");

        using var refresh = await fixture.MainApi.SendAsync(
            refreshRequest, TestContext.Current.CancellationToken);
        Assert.True(refresh.IsSuccessStatusCode,
            $"Refresh в Main не удался: {(int)refresh.StatusCode}");

        var body = await refresh.Content.ReadFromJsonAsync<JsonElement>(
            TestContext.Current.CancellationToken);
        var accessToken = body.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(accessToken),
            "Main не вернул accessToken после refresh");

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

    [Fact]
    public async Task Payments_RejectsMalformedToken()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ProtectedEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-jwt");

        using var response = await fixture.PaymentsApi.SendAsync(
            request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<string> GetMainAccessTokenAsync()
    {
        var email = await RegisterUserAsync();

        var body = await LoginAsync(email);
        var accessToken = body.GetProperty("accessToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(accessToken), "Main не вернул accessToken");
        return accessToken!;
    }

    private async Task<string> RegisterUserAsync()
    {
        var email = $"e2e-{Guid.NewGuid():N}@test.local";

        using var register = await fixture.MainApi.PostAsJsonAsync(
            "api/auth/register",
            new { name = "E2E", email, password = Password });
        Assert.True(register.IsSuccessStatusCode,
            $"Регистрация в Main не удалась: {(int)register.StatusCode}");

        return email;
    }

    private async Task<JsonElement> LoginAsync(string email)
    {
        using var login = await fixture.MainApi.PostAsJsonAsync(
            "api/auth/login",
            new { email, password = Password },
            TestContext.Current.CancellationToken);
        Assert.True(login.IsSuccessStatusCode,
            $"Логин в Main не удался: {(int)login.StatusCode}");

        return await login.Content.ReadFromJsonAsync<JsonElement>();
    }

    private static string? GetRefreshCookie(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            return null;
        }

        foreach (var cookie in cookies)
        {
            var pair = cookie.Split(';', 2)[0];
            var separator = pair.IndexOf('=');
            if (separator > 0 && pair[..separator].Trim() == RefreshCookieName)
            {
                return pair[(separator + 1)..].Trim();
            }
        }

        return null;
    }
}
