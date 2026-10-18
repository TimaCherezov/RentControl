using System.Security.Claims;

namespace Main.Api.Auth.CurrentUser;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid GetUserId()
    {
        var idStr = httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(idStr, out var id))
        {
            throw new UnauthorizedAccessException("Missing/invalid NameIdentifier claim.");
        }

        return id;
    }
}
