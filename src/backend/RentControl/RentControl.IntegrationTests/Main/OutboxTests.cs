using System.Net;
using System.Net.Http.Headers;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RentControl.Common.IntegrationEvents;
using RentControl.IntegrationTests.Main.Infrastructure;

namespace RentControl.IntegrationTests.Main;

[Collection(MainApiCollection.Name)]
public class OutboxTests(MainApiFixture fixture)
{
    [Fact]
    public async Task PublishOrderCreated_GoesThroughOutbox_AndDeliveredToRabbit()
    {
        var probe = new OrderCreatedProbe();
        var probeHost = StartProbeHost(fixture.RabbitMqConnectionString, probe);
        await probeHost.StartAsync();

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/debug/publish-order-created");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer", fixture.CreateAccessToken(Guid.NewGuid()));

            var response = await fixture.Client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode,
                $"POST вернул {(int)response.StatusCode}: {responseBody}");

            var body = System.Text.Json.JsonSerializer.Deserialize<PublishResponse>(
                responseBody,
                new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
            Assert.NotNull(body);

            // Assert 1: сообщение доставлено из outbox в RabbitMQ и получено пробой.
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var delivered = await probe.Received.WaitAsync(cts.Token);

            Assert.Equal(body.BookingId, delivered.BookingId);
            Assert.Equal(1000m, delivered.Amount);

            // Assert 2: publish действительно шёл через outbox — запись в OutboxMessage существует
            // (delivery не удаляет её сразу; это отличает outbox от прямой публикации в брокер).
            await using var db = fixture.CreateDbContext();
            Assert.True(await db.Set<OutboxMessage>().AnyAsync());
        }
        finally
        {
            await probeHost.StopAsync();
            probeHost.Dispose();
        }
    }

    private sealed record PublishResponse(Guid BookingId);

    private static IHost StartProbeHost(string rabbitMqConnectionString, OrderCreatedProbe probe)
    {
        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Services.AddLogging();
        hostBuilder.Services.AddSingleton(probe);

        hostBuilder.Services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<OrderCreatedProbeConsumer>();

            cfg.UsingRabbitMq((context, rmq) =>
            {
                rmq.Host(rabbitMqConnectionString);
                rmq.ReceiveEndpoint("outbox-tests-probe", e =>
                {
                    e.ConfigureConsumer<OrderCreatedProbeConsumer>(context);
                });
            });
        });

        return hostBuilder.Build();
    }

    private sealed class OrderCreatedProbe : IConsumer<OrderCreated>
    {
        private readonly TaskCompletionSource<OrderCreated> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<OrderCreated> Received => _completion.Task;

        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            _completion.TrySetResult(context.Message);
            return Task.CompletedTask;
        }
    }

    private sealed class OrderCreatedProbeConsumer(OrderCreatedProbe probe) : IConsumer<OrderCreated>
    {
        public Task Consume(ConsumeContext<OrderCreated> context) => probe.Consume(context);
    }
}
