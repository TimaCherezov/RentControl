using System.Security.Claims;
using RentControl.ServiceDefaults.Exceptions;

namespace Main.Api.Features.Auth.CurrentUser;

public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid GetUserId()
    {
        var idValue = httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(idValue, out var userId))
        {
            throw new UnauthorizedRequestException("Missing/invalid NameIdentifier claim.");
        }

        return userId;
    }
}
