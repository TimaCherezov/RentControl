using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RentControl.E2ETests;

/// <summary>
/// E2E-сценарии взаимодействия систем: Payments авторизует JWT, выданные Main.
/// Внутреннее поведение самих сервисов (валидация регистра, ротация refresh-токенов
/// и т.п.) здесь не тестируется — для этого интеграционные тесты сервисов.
/// </summary>
[Collection(AppHostCollection.Name)]
public sealed class CrossServiceAuthTests(AppHostFixture fixture)
{
    private const string Issuer = "RentControl";
    private const string Audience = "RentControl";
    private const string ProtectedEndpoint = "weatherforecast";

    [Fact]
    public async Task Payments_accepts_token_issued_by_main()
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
    public async Task Payments_rejects_token_signed_by_foreign_key()
    {
        // Токен оформлен как настоящий (тот же issuer/audience, валидный exp),
        // но подписан ключом, которого нет в JWKS Main.
        var forgedToken = ForgedJwt.Create(Issuer, Audience);

        using var request = new HttpRequestMessage(HttpMethod.Get, ProtectedEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", forgedToken);

        using var response = await fixture.PaymentsApi.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    /// <summary>
    /// Получает access token у Main.Api: register + login.
    /// Это подготовка данных, а не проверка поведения Main.
    /// </summary>
    private async Task<string> GetMainAccessTokenAsync()
    {
        // Уникальный email: том postgres персистентный, данные переживают прогоны.
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
