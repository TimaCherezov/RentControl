using Main.Api.Auth;
using Main.Api.Auth.Refresh;
using Main.Model;
using Main.Model.Models;
using Microsoft.Extensions.Options;
using RentControl.Auth;

namespace Main.Api.Auth.Refresh;

public class RefreshTokenService(
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtSettings) : IRefreshTokenService
{
    public async Task<TokenPairResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = jwtTokenService.HashRefreshToken(refreshToken);
        var currentToken = await unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (currentToken is not { RevokedAt: null } || currentToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await unitOfWork.Users.GetByIdAsync(currentToken.UserId, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var newAccess = jwtTokenService.GenerateToken(user.Id);
        var newRefresh = jwtTokenService.GenerateRefreshToken();
        var newRefreshHash = jwtTokenService.HashRefreshToken(newRefresh);

        currentToken.RevokedAt = DateTime.UtcNow;
        unitOfWork.RefreshTokens.Update(currentToken);

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshHash,
            ExpiresAt = DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiry)
        };
        unitOfWork.RefreshTokens.Add(newRefreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenPairResponse(newAccess, newRefresh);
    }
}
