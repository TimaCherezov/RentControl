using Main.Model;
using Main.Model.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RentControl.Auth;

namespace Main.Api.Auth.Login;

public class LoginUserService(
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService,
    IOptions<JwtSettings> jwtSettings) : ILoginUserService
{
    public async Task<TokenPairResponse> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
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
            ExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpiryDays)
        };

        unitOfWork.RefreshTokens.Add(refreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenPairResponse(jwt, refresh);
    }
}
