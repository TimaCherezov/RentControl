using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RentControl.E2ETests;

/// <summary>
/// JWT, корректный по форме и claims (issuer/audience как у настоящих токенов),
/// но подписанный собственным ключом теста, а не ключом Main.Api.
/// Если Payments проверяет подпись через JWKS — такой токен должен быть отвергнут.
/// </summary>
internal static class ForgedJwt
{
    public static string Create(string issuer, string audience)
    {
        using var rsa = RSA.Create(2048);

        var header = Base64Url(Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(new { alg = "RS256", typ = "JWT" })));

        var payload = Base64Url(Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(new
            {
                sub = Guid.NewGuid().ToString(),
                iss = issuer,
                aud = audience,
                iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                exp = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds()
            })));

        var signingInput = $"{header}.{payload}";
        var signature = Base64Url(rsa.SignData(
            Encoding.ASCII.GetBytes(signingInput),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1));

        return $"{signingInput}.{signature}";
    }

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
