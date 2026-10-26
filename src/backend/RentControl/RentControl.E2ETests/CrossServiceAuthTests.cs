using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
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

        using var response = await fixture.PaymentsApi.SendAsync(request);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Ожидался 200 от Payments для токена, выданного Main, но пришел {response.StatusCode}");
    }

    [Fact]
    public async Task Payments_RejectsTokenSignedByForeignKey()
    {
        var forgedToken = CreateTokenSignedByForeignKey(Issuer, Audience);

        using var request = new HttpRequestMessage(HttpMethod.Get, ProtectedEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", forgedToken);

        using var response = await fixture.PaymentsApi.SendAsync(request);

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

    private static string CreateTokenSignedByForeignKey(string issuer, string audience)
    {
        using var rsa = RSA.Create(2048);

        var header = Base64Url(Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(new { alg = "RS256", typ = "JWT" })));

        var payload = Base64Url(Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(new
            {
                sub = Guid.NewGuid().ToString(),
                iss = issuer,
                aud = audience,
                iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                exp = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds()
            })));

        var signingInput = $"{header}.{payload}";
        var signature = Base64Url(rsa.SignData(
            Encoding.ASCII.GetBytes(signingInput),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1));

        return $"{signingInput}.{signature}";
    }

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
