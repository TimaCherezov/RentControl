using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Realtime.Api.Hubs;

[Authorize]
public sealed class BookingHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new HubException(
                "JWT doesn't contain a valid NameIdentifier claim.");
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GetUserGroup(parsedUserId));

        await base.OnConnectedAsync();
    }

    public static string GetUserGroup(Guid userId)
        => $"user:{userId}";
}
