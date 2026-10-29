using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Main.Api.Auth;
using Main.Api.Auth.Login;
using Main.Api.Auth.Register;
using RentControl.IntegrationTests.Main.Infrastructure;

namespace RentControl.IntegrationTests.Main;

[Collection(MainApiCollection.Name)]
public class AuthFlowTests(MainApiFixture fixture)
{
    [Fact]
    public async Task Register_Login_ThenCurrentUser_ReturnsRegisteredUser()
    {
        var email = $"int-{Guid.NewGuid():N}@test.local";
        var password = "P@ssw0rd-123";

        var registerResponse = await fixture.Client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterUserRequest("Интеграционный Тест", email, password));

        Assert.True(
            registerResponse.StatusCode == HttpStatusCode.OK,
            $"Регистрация не удалась ({registerResponse.StatusCode}): {await registerResponse.Content.ReadAsStringAsync()}");

        var loginResponse = await fixture.Client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginUserRequest(email, password));

        Assert.True(
            loginResponse.StatusCode == HttpStatusCode.OK,
            $"Логин не удался ({loginResponse.StatusCode}): {await loginResponse.Content.ReadAsStringAsync()}");

        var tokens = await loginResponse.Content.ReadFromJsonAsync<TokenPairResponse>();
        Assert.False(string.IsNullOrWhiteSpace(tokens?.AccessToken), "Main не вернул accessToken при логине");

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/current-user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var currentUserResponse = await fixture.Client.SendAsync(request);

        Assert.True(
            currentUserResponse.StatusCode == HttpStatusCode.OK,
            $"current-user не удался ({currentUserResponse.StatusCode}): {await currentUserResponse.Content.ReadAsStringAsync()}");

        var user = await currentUserResponse.Content.ReadFromJsonAsync<UserResponse>();
        Assert.Equal(email, user?.Email);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        var email = $"int-{Guid.NewGuid():N}@test.local";

        var first = await fixture.Client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterUserRequest("Первый", email, "P@ssw0rd-123"));
        Assert.True(
            first.StatusCode == HttpStatusCode.OK,
            $"Первая регистрация не удалась ({first.StatusCode}): {await first.Content.ReadAsStringAsync()}");

        var second = await fixture.Client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterUserRequest("Второй", email, "Другой-пароль-123"));

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var email = $"int-{Guid.NewGuid():N}@test.local";

        var register = await fixture.Client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterUserRequest("Интеграционный Тест", email, "P@ssw0rd-123"));
        Assert.True(
            register.StatusCode == HttpStatusCode.OK,
            $"Регистрация не удалась ({register.StatusCode}): {await register.Content.ReadAsStringAsync()}");

        var login = await fixture.Client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginUserRequest(email, "Не-тот-пароль"));

        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }

    [Fact]
    public async Task CurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        var response = await fixture.Client.GetAsync("/api/auth/current-user");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CurrentUser_WithInvalidToken_ReturnsUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/current-user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "не-jwt-строка");

        var response = await fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CurrentUser_WithExpiredToken_ReturnsUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/current-user");
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            // Час, а не пять минут: у JwtBearer дефолтный ClockSkew = 5 минут,
            // на границе токен может быть признан валидным.
            fixture.CreateAccessToken(Guid.NewGuid(), TimeSpan.FromHours(-1)));

        var response = await fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
