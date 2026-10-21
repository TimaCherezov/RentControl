namespace Main.Api.Auth.Refresh;

public interface IRefreshTokenService
{
    Task<TokenPairResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
}
