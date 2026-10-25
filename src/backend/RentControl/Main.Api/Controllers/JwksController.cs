using Microsoft.AspNetCore.Mvc;
using RentControl.ServiceDefaults;

namespace Main.Api.Controllers;

[ApiController]
public class JwksController(JwtSigningKeyProvider signingKeys) : ControllerBase
{
    [HttpGet("/.well-known/jwks.json")]
    public IActionResult GetJwks() => Ok(new { keys = new[] { signingKeys.ToJwk() } });
}
