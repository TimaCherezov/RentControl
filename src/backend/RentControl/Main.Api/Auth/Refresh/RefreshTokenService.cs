using Main.Api.Auth.Refresh;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using Microsoft.Extensions.Options;
using RentControl.Auth;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Auth.Refresh;

public class RefreshTokenService(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtSettings) : IRefreshTokenService
{
    public Task<TokenPairResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return uowFactory.ExecuteAsync(async uow =>
        {
            var tokenHash = jwtTokenService.HashRefreshToken(refreshToken);
            var currentToken = await uow.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);
            if (currentToken is not { RevokedAt: null } || currentToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var user = await uow.Users.GetByIdAsync(currentToken.UserId, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var newAccess = jwtTokenService.GenerateToken(user.Id);
            var newRefresh = jwtTokenService.GenerateRefreshToken();
            var newRefreshHash = jwtTokenService.HashRefreshToken(newRefresh);

            currentToken.RevokedAt = DateTime.UtcNow;
            uow.RefreshTokens.Update(currentToken);

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshHash,
                ExpiresAt = DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiry)
            };
            uow.RefreshTokens.Add(newRefreshToken);

            return new TokenPairResponse(newAccess, newRefresh);
        }, cancellationToken);
    }
}
