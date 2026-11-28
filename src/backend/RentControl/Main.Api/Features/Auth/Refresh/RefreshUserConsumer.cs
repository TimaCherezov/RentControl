using RentControl.ServiceDefaults.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using RentControl.Common.UnitOfWork;
using RentControl.ServiceDefaults;

namespace Main.Api.Features.Auth.Refresh;

public class RefreshUserConsumer(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtSettings) : IConsumer<RefreshUserRequest>
{
    public async Task Consume(ConsumeContext<RefreshUserRequest> context)
    {
        var response = await uowFactory.ExecuteAsync(async uow =>
        {
            var request = context.Message;
            var tokenHash = jwtTokenService.HashRefreshToken(request.RefreshToken);
            var currentToken = await uow.RefreshTokens.GetByTokenHashAsync(
                tokenHash,
                context.CancellationToken);

            if (currentToken is not { RevokedAt: null } || currentToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedRequestException("Invalid or expired refresh token");
            }

            var user = await uow.Users.GetByIdAsync(
                currentToken.UserId,
                context.CancellationToken);
            if (user is null)
            {
                throw new UnauthorizedRequestException("User not found");
            }

            var accessToken = jwtTokenService.GenerateToken(user.Id, user.Role);
            var refreshToken = jwtTokenService.GenerateRefreshToken();

            currentToken.RevokedAt = DateTime.UtcNow;

            uow.RefreshTokens.Add(new RefreshToken
            {
                User = user,
                UserId = user.Id,
                TokenHash = jwtTokenService.HashRefreshToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiry)
            });

            return new TokenPairResponse(accessToken, refreshToken);
        }, context.CancellationToken);

        await context.RespondAsync(response);
    }
}
