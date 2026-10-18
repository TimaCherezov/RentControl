using Main.Model;

namespace Main.Api.Auth.Logout;

public class LogoutService(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService) : ILogoutService
{
    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = jwtTokenService.HashRefreshToken(refreshToken);
        var token = await unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (token is not { RevokedAt: null })
        {
            return;
        }

        token.RevokedAt = DateTime.UtcNow;
        unitOfWork.RefreshTokens.Update(token);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
