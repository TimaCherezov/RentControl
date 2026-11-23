using Main.Api.Features.Auth;
using Main.Api.Features.Auth.CurrentUser;
using Main.Api.Features.Auth.Login;
using Main.Api.Features.Auth.Logout;
using Main.Api.Features.Auth.Refresh;
using Main.Api.Features.Auth.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RentControl.ServiceDefaults;
using MassTransit.Mediator;

namespace Main.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(
    IMediator mediator,
    IOptions<JwtOptions> jwtSettings) : ControllerBase
{
    private const string RefreshCookieName = "refresh_token";
    private const string CookiePath = "/api/auth";

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var client = mediator.CreateRequestClient<RegisterUserRequest>();
        var response = await client.GetResponse<UserResponse>(request, cancellationToken);
        return Ok(response.Message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var client = mediator.CreateRequestClient<LoginUserRequest>();
        var response = await client.GetResponse<TokenPairResponse>(request, cancellationToken);
        SetRefreshCookie(response.Message.RefreshToken);
        return Ok(response.Message);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenPairResponse>> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { error = "Refresh token is missing" });
        }

        var client = mediator.CreateRequestClient<RefreshUserRequest>();
        var response = await client.GetResponse<TokenPairResponse>(
            new RefreshUserRequest(refreshToken),
            cancellationToken);
        SetRefreshCookie(response.Message.RefreshToken);
        return Ok(response.Message);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await mediator.Send(new LogoutUserRequest(refreshToken), cancellationToken);
        }

        Response.Cookies.Delete(RefreshCookieName, RefreshCookieOptions);
        return NoContent();
    }

    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> CurrentUser(CancellationToken cancellationToken)
    {
        var client = mediator.CreateRequestClient<CurrentUserRequest>();
        var response = await client.GetResponse<UserResponse>(
            new CurrentUserRequest(),
            cancellationToken);
        return Ok(response.Message);
    }

    private void SetRefreshCookie(string token) =>
        Response.Cookies.Append(RefreshCookieName, token, RefreshCookieOptions);

    private CookieOptions RefreshCookieOptions =>
        new()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Path = CookiePath,
            Expires = DateTimeOffset.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiry)
        };
}
