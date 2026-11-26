using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RentControl.E2ETests.Utils;

public static class TestUsers
{
    public const string Password = "e2e-password-123";

    public static string NewEmail() => $"e2e-{Guid.NewGuid():N}@test.local";

    public static async Task RegisterAsync(
        HttpClient mainApi,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        using var register = await mainApi.PostAsJsonAsync(
            "api/auth/register",
            new { name = "E2E", email, password },
            cancellationToken);
        Assert.True(register.IsSuccessStatusCode,
            $"Регистрация в Main не удалась: {(int)register.StatusCode} {await register.Content.ReadAsStringAsync(cancellationToken)}");
    }

    public static async Task<JsonElement> LoginAsync(
        HttpClient mainApi,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        using var login = await mainApi.PostAsJsonAsync(
            "api/auth/login",
            new { email, password },
            cancellationToken);
        Assert.True(login.IsSuccessStatusCode,
            $"Логин в Main не удался: {(int)login.StatusCode} {await login.Content.ReadAsStringAsync(cancellationToken)}");

        return await login.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
    }

    public static async Task<string> GetAccessTokenAsync(
        HttpClient mainApi,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var body = await LoginAsync(mainApi, email, password, cancellationToken);
        var accessToken = body.GetProperty("accessToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(accessToken), "Main не вернул accessToken");
        return accessToken!;
    }

    public static async Task<string> RegisterAndGetAccessTokenAsync(
        HttpClient mainApi,
        CancellationToken cancellationToken)
    {
        var email = NewEmail();
        await RegisterAsync(mainApi, email, Password, cancellationToken);
        return await GetAccessTokenAsync(mainApi, email, Password, cancellationToken);
    }

    public static AuthenticationHeaderValue Bearer(string accessToken) => new("Bearer", accessToken);
}
