using RentControl.ServiceDefaults.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RentControl.Common.UnitOfWork;
using RentControl.ServiceDefaults;

namespace Main.Api.Features.Auth.Login;

public class LoginUserConsumer(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtSettings) : IConsumer<LoginUserRequest>
{
    public async Task Consume(ConsumeContext<LoginUserRequest> context)
    {
        var response = await uowFactory.ExecuteAsync(async uow =>
        {
            var request = context.Message;
            var user = await uow.Users.GetByEmailAsync(
                request.Email,
                context.CancellationToken);
            if (user is null)
            {
                throw new UnauthorizedRequestException("Invalid email");
            }

            var passwordResult = passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);
            if (passwordResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedRequestException("Invalid Password");
            }

            var accessToken = jwtTokenService.GenerateToken(user.Id, user.Role);
            var refreshToken = jwtTokenService.GenerateRefreshToken();

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
