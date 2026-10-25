using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace RentControl.ServiceDefaults;

public class JwtSigningKeyProvider
{
    public JwtSigningKeyProvider(IOptions<JwtOptions> options)
    {
        var pem = options.Value.PrivateKeyPem;
        if (string.IsNullOrWhiteSpace(pem))
        {
            throw new InvalidOperationException(
                $"'{JwtOptions.Section}:PrivateKeyPem' is not configured.");
        }

        var rsa = RSA.Create();
        rsa.ImportFromPem(pem);

        Key = new RsaSecurityKey(rsa) { KeyId = ComputeKid(rsa) };
        Credentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256);
    }

    public RsaSecurityKey Key { get; }

    public SigningCredentials Credentials { get; }

    public JwkDto ToJwk()
    {
        var parameters = Key.Rsa!.ExportParameters(false);

        return new JwkDto(
            Kty: "RSA",
            Use: "sig",
            Alg: SecurityAlgorithms.RsaSha256,
            Kid: Key.KeyId,
            N: Base64UrlEncoder.Encode(parameters.Modulus!),
            E: Base64UrlEncoder.Encode(parameters.Exponent!));
    }

    private static string ComputeKid(RSA rsa)
    {
        var modulus = rsa.ExportParameters(false).Modulus!;
        return Base64UrlEncoder.Encode(SHA256.HashData(modulus));
    }
}
