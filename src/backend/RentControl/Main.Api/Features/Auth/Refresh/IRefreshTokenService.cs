namespace Main.Api.Features.Auth.Refresh;

public interface IRefreshTokenService
{
    Task<TokenPairResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
}
