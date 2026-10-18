namespace Main.Api.Auth;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
}
