using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
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

        Assert.Equal(HttpStatusCode.OK, currentUserResponse.StatusCode);

        var user = await currentUserResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(email, user.GetProperty("email").GetString());
    }
}
