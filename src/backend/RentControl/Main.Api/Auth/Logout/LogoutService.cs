using Main.Model.UnitOfWork;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Auth.Logout;

public class LogoutService(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, IJwtTokenService jwtTokenService) : ILogoutService
{
    public Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return uowFactory.ExecuteAsync(async uow =>
        {
            var tokenHash = jwtTokenService.HashRefreshToken(refreshToken);
            var token = await uow.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);
            if (token is not { RevokedAt: null })
            {
                return;
            }

            token.RevokedAt = DateTime.UtcNow;
            uow.RefreshTokens.Update(token);
        }, cancellationToken);
    }
}
