using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RentControl.ServiceDefaults;

namespace RentControl.IntegrationTests.Infrastructure;

/// <summary>
/// Эфемерная RSA-пара: PEM уходит в конфиг сервиса (Jwt:PrivateKeyPem),
/// ключом выпускаются тестовые access-токены и JWKS-документ.
/// </summary>
public sealed class TestJwtKeys : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RSA _rsa;
    private readonly string _privateKeyPem;
    private readonly JwtSigningKeyProvider _signingKeys;

    public TestJwtKeys()
    {
        _rsa = RSA.Create(2048);
        _privateKeyPem = _rsa.ExportPkcs8PrivateKeyPem();
        _signingKeys = new JwtSigningKeyProvider(
            Options.Create(new JwtOptions { PrivateKeyPem = _privateKeyPem }));
    }

    public string PrivateKeyPem => _privateKeyPem;

    public void Dispose() => _rsa.Dispose();

    /// <summary>
    /// Выпускает access-токен в том же формате, что и Main.Api: RS256,
    /// идентификатор пользователя в клейме nameid (ClaimTypes.NameIdentifier
    /// после outbound-маппинга JwtSecurityTokenHandler).
    /// </summary>
    public string CreateAccessToken(
        Guid userId,
        string issuer,
        string audience,
        TimeSpan? lifetime = null)
    {
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
            expires: DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(15)),
            signingCredentials: _signingKeys.Credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>JWKS-документ с публичным ключом — для заглушки /.well-known/jwks.json.</summary>
    public string ToJwksJson() =>
        JsonSerializer.Serialize(new { keys = new[] { _signingKeys.ToJwk() } }, JsonOptions);
}
