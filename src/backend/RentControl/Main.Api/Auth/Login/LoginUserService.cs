using Main.Model.Models;
using Main.Model.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RentControl.Auth;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Auth.Login;

public class LoginUserService(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtSettings) : ILoginUserService
{
    public Task<TokenPairResponse> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        return uowFactory.ExecuteAsync(async uow =>
        {
            var user = await uow.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid email");
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid Password");
            }

            var jwt = jwtTokenService.GenerateToken(user.Id);
            var refresh = jwtTokenService.GenerateRefreshToken();
            var refreshHash = jwtTokenService.HashRefreshToken(refresh);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshHash,
                ExpiresAt = DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiry)
            };

            uow.RefreshTokens.Add(refreshToken);

            return new TokenPairResponse(jwt, refresh);
        }, cancellationToken);
    }
}
