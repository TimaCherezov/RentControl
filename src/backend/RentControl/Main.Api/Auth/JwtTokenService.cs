using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RentControl.Auth;

namespace Main.Api.Auth;

public class JwtTokenService(
    JwtSigningKeyProvider signingKeys,
    IOptions<JwtOptions> jwtOptions) : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateToken(Guid userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrEmpty(_jwtOptions.Issuer) ? null : _jwtOptions.Issuer,
            audience: string.IsNullOrEmpty(_jwtOptions.Audience) ? null : _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_jwtOptions.Expiry),
            signingCredentials: signingKeys.Credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return WebEncoders.Base64UrlEncode(randomBytes);
    }

    public string HashRefreshToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}
