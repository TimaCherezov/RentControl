using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Auth.Logout;

public class LogoutUserConsumer(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IJwtTokenService jwtTokenService) : IConsumer<LogoutUserRequest>
{
    public Task Consume(ConsumeContext<LogoutUserRequest> context)
    {
        return uowFactory.ExecuteAsync(async uow =>
        {
            var tokenHash = jwtTokenService.HashRefreshToken(context.Message.RefreshToken);
            var token = await uow.RefreshTokens.GetByTokenHashAsync(
                tokenHash,
                context.CancellationToken);
            if (token is { RevokedAt: null })
            {
                token.RevokedAt = DateTime.UtcNow;
            }
        }, context.CancellationToken);
    }
}
