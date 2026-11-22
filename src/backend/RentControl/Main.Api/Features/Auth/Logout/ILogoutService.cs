namespace Main.Api.Features.Auth.Logout;

public interface ILogoutService
{
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}
