using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Realtime.Api.Hubs;
using Realtime.Model;
using RentControl.Common.IntegrationEvents;

namespace Realtime.Api.Consumers;

public sealed class OrderChangedConsumer(
    IHubContext<BookingHub> hubContext,
    ILogger<OrderChangedConsumer> logger)
    : IConsumer<OrderChanged>
{
    public async Task Consume(
        ConsumeContext<OrderChanged> context)
    {
        var message = context.Message;

        await hubContext.Clients
            .Group(BookingHub.GetUserGroup(message.UserId))
            .SendAsync(
                "BookingChanged",
                new BookingChangedNotification(
                    message.BookingId,
                    message.Status),
                context.CancellationToken);

        logger.LogInformation(
            "Sent BookingChanged notification for booking {BookingId} to user {UserId}",
            message.BookingId,
            message.UserId);
    }
}
