using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Main.Model.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RentControl.Common.IntegrationEvents;
using RentControl.IntegrationTests.Main.Infrastructure;

namespace RentControl.IntegrationTests.Main;

[Collection(MainApiCollection.Name)]
public class OutboxTests(MainApiFixture fixture)
{
    [Fact]
    public async Task CreateBooking_WritesBookingAndOrderCreatedAtomically_DeliveredToRabbit()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var user = new User { Id = Guid.NewGuid(), Name = "test", Email = $"{suffix}@test.local", PasswordHash = "hash" };
        var location = new Location { Id = Guid.NewGuid(), Name = "test", Address = "test" };
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "test",
            MarketValue = 1000m,
            DailyRate = 100m,
            HourlyRate = 10m,
            DepositOverride = 500m,
        };
        var unit = new ProductUnit
        {
            Id = Guid.NewGuid(),
            Product = product,
            State = StateEnum.Good,
            SerialNumber = suffix,
            PurchaseDate = DateOnly.FromDateTime(DateTime.UtcNow),
            PurchasePrice = 1000m,
            Location = location,
        };

        await using (var db = fixture.CreateDbContext())
        {
            db.AddRange(user, location, product, unit);
            await db.SaveChangesAsync();
        }

        var probe = new OrderCreatedProbe();
        var probeHost = StartProbeHost(fixture.RabbitMqConnectionString, probe);
        await probeHost.StartAsync();

        try
        {
            const decimal depositAmount = 500m;
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/bookings");
            request.Content = JsonContent.Create(new
            {
                productId = product.Id,
                locationId = location.Id,
                userId = user.Id,
                startAt = DateTimeOffset.UtcNow.AddDays(1),
                endAt = DateTimeOffset.UtcNow.AddDays(2),
            });
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer", fixture.CreateAccessToken(Guid.NewGuid()));

            var response = await fixture.Client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode,
                $"POST /api/bookings вернул {(int)response.StatusCode}: {responseBody}");

            var created = JsonSerializer.Deserialize<BookingResponse>(
                responseBody,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
            Assert.NotNull(created);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var delivered = await probe.Received.WaitAsync(cts.Token);

            Assert.Equal(created.Id, delivered.BookingId);
            Assert.Equal(depositAmount, delivered.Amount);
        }
        finally
        {
            await probeHost.StopAsync();
            probeHost.Dispose();
        }
    }

    private sealed record BookingResponse(Guid Id);

    private static IHost StartProbeHost(string rabbitMqConnectionString, OrderCreatedProbe probe)
    {
        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Services.AddLogging();
        hostBuilder.Services.AddSingleton(probe);

        hostBuilder.Services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<OrderCreatedProbe>();

            cfg.UsingRabbitMq((context, rmq) =>
            {
                rmq.Host(rabbitMqConnectionString);
                rmq.ReceiveEndpoint("outbox-tests-probe", e =>
                {
                    e.ConfigureConsumer<OrderCreatedProbe>(context);
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
}
