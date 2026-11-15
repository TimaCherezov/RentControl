using Main.Model.Models;

namespace Main.Api.Auth;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, RoleEnum role);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
}
