using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Realtime.Model;
using RentControl.Common.IntegrationEvents;
using RentControl.IntegrationTests.Infrastructure;
using RentControl.IntegrationTests.Realtime.Infrastructure;

namespace RentControl.IntegrationTests.Realtime;

[Collection(RealtimeApiCollection.Name)]
public class OrderChangedConsumerTests(RealtimeApiFixture fixture)
{
    [Fact]
    public async Task OrderChanged_SendsBookingChangedToUserGroup()
    {
        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        var publisher = StartPublisher(fixture.RabbitMqConnectionString);
        await publisher.StartAsync();

        try
        {
            var bus = publisher.Services.GetRequiredService<IBus>();
            await bus.Publish(new OrderChanged(
                bookingId,
                userId,
                "Confirmed"));

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(30));

            var notification = await fixture.NotificationProbe.Reader
                .ReadAsync(cts.Token);

            Assert.Equal(
                $"group:user:{userId}",
                notification.Group);
            Assert.Equal("BookingChanged", notification.Method);

            var json = JsonSerializer.Serialize(
                notification.Payload,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
            var payload = JsonSerializer.Deserialize<BookingChangedNotification>(
                json,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            Assert.NotNull(payload);
            Assert.Equal(bookingId, payload.BookingId);
            Assert.Equal("Confirmed", payload.Status);
        }
        finally
        {
            await publisher.StopAsync();
            publisher.Dispose();
        }
    }

    private static IHost StartPublisher(
        string rabbitMqConnectionString)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddLogging();
        builder.Services.WaitMassTransitUntilStarted();

        builder.Services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((_, rabbitMq) =>
            {
                rabbitMq.Host(rabbitMqConnectionString);
            });
        });

        return builder.Build();
    }
}
