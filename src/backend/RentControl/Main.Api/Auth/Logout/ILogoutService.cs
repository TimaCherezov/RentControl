namespace Main.Api.Auth.Logout;

public interface ILogoutService
{
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}
