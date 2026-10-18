using Main.Api.Auth;
using Main.Api.Auth.CurrentUser;
using Main.Api.Auth.Login;
using Main.Api.Auth.Logout;
using Main.Api.Auth.Refresh;
using Main.Api.Auth.Register;
using Main.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RentControl.Auth;

namespace Main.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(
    IRegisterUserService registerUserService,
    ILoginUserService loginUserService,
    IRefreshTokenService refreshTokenService,
    ILogoutService logoutService,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IOptions<JwtSettings> jwtSettings) : ControllerBase
{
    private const string RefreshCookieName = "refresh_token";
    private const string CookiePath = "/api/auth";

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var response = await registerUserService.RegisterAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var response = await loginUserService.LoginAsync(request, cancellationToken);
        SetRefreshCookie(response.RefreshToken);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenPairResponse>> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { error = "Refresh token is missing" });
        }

        var result = await refreshTokenService.RefreshAsync(refreshToken, cancellationToken);
        SetRefreshCookie(result.RefreshToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await logoutService.LogoutAsync(refreshToken, cancellationToken);
        }

        Response.Cookies.Delete(RefreshCookieName, RefreshCookieOptions);
        return NoContent();
    }

    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> CurrentUser(CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Unauthorized(new { error = "User not found" });
        }

        return Ok(new UserResponse(user.Id, user.Name, user.Email));
    }

    private void SetRefreshCookie(string token) =>
        Response.Cookies.Append(RefreshCookieName, token, RefreshCookieOptions);

    private CookieOptions RefreshCookieOptions =>
        new()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Path = CookiePath,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpiryDays)
        };
}
