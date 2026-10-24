using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RentControl.Auth;

namespace Main.Api.Controllers;

[ApiController]
public class JwksController(JwtSigningKeyProvider signingKeys, IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    [HttpGet("/.well-known/jwks.json")]
    public IActionResult GetJwks() => Ok(new { keys = new[] { signingKeys.ToJwk() } });

    [HttpGet("/.well-known/openid-configuration")]
    public IActionResult GetConfiguration()
    {
        var settings = jwtOptions.Value;
        var publicUrl = settings.PublicUrl;

        if (string.IsNullOrWhiteSpace(publicUrl))
        {
            throw new InvalidOperationException(
                $"'{JwtOptions.Section}:PublicUrl' is not configured.");
        }

        return Ok(new
        {
            jwks_uri = $"{publicUrl.TrimEnd('/')}/.well-known/jwks.json"
        });
    }
}
