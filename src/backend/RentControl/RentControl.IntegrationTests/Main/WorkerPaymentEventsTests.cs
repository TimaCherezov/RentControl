extern alias MainWorker;

using System.Threading.Channels;
using Main.Model;
using Main.Model.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RentControl.Common.IntegrationEvents;
using RentControl.IntegrationTests.Main.Infrastructure;
using MainWorker.Main.Worker;

namespace RentControl.IntegrationTests.Main;

[Collection(MainApiCollection.Name)]
public class WorkerPaymentEventsTests(MainApiFixture fixture)
{
    [Fact]
    public async Task InvoiceCreated_ThenPaymentSucceeded_UpdatesBooking_AndPublishesOrderChanged()
    {
        var bookingId = await SeedBookingAsync();

        var workerBuilder = Host.CreateApplicationBuilder();
        workerBuilder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:maindb"] = fixture.ConnectionString,
            ["ConnectionStrings:rabbitmq"] = fixture.RabbitMqConnectionString,
        });
        workerBuilder.Services.AddLogging();
        workerBuilder.AddMainDatabase("maindb", disableRetry: true);
        workerBuilder.AddMainWorkerBus();
        var worker = workerBuilder.Build();
        await worker.StartAsync();

        var probe = new OrderChangedProbe();
        var probeHost = StartProbeHost(fixture.RabbitMqConnectionString, probe);
        await probeHost.StartAsync();
        try
        {
            var bus = probeHost.Services.GetRequiredService<IBus>();

            await bus.Publish(new InvoiceCreated(bookingId));
            var first = await probe.NextAsync(TimeSpan.FromSeconds(30));
            Assert.Equal(bookingId, first.BookingId);
            Assert.Equal(nameof(BookingStatusEnum.Pending), first.Status);

            await using (var db = fixture.CreateDbContext())
            {
                var booking = await db.Bookings.AsNoTracking().SingleAsync(b => b.Id == bookingId);
                Assert.True(booking.IsInvoiceCreated);
            }

            await bus.Publish(new PaymentSucceeded(bookingId));
            var second = await probe.NextAsync(TimeSpan.FromSeconds(30));
            Assert.Equal(bookingId, second.BookingId);
            Assert.Equal(nameof(BookingStatusEnum.Confirmed), second.Status);

            await using (var db = fixture.CreateDbContext())
            {
                var booking = await db.Bookings.AsNoTracking().SingleAsync(b => b.Id == bookingId);
                Assert.Equal(BookingStatusEnum.Confirmed, booking.BookingStatus);
            }
        }
        finally
        {
            await probeHost.StopAsync();
            probeHost.Dispose();
            await worker.StopAsync();
            worker.Dispose();
        }
    }

    private async Task<Guid> SeedBookingAsync()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var user = new User { Id = Guid.NewGuid(), Name = "worker-test", Email = $"wt-{suffix}@test.local", PasswordHash = "hash" };
        var location = new Location { Id = Guid.NewGuid(), Name = "worker-test", Address = "worker-test" };
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "worker-test",
            MarketValue = 1000m,
            DailyRate = 100m,
            HourlyRate = 10m,
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
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ProductUnit = unit,
            Location = location,
            StartAt = DateTimeOffset.UtcNow.AddDays(1),
            EndAt = DateTimeOffset.UtcNow.AddDays(2),
            DepositAmount = 100m,
            BookingStatus = BookingStatusEnum.Pending,
            StatusChangedAt = DateTimeOffset.UtcNow,
            User = user,
        };

        await using var db = fixture.CreateDbContext();
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        return booking.Id;
    }

    private static IHost StartProbeHost(string rabbitMqConnectionString, OrderChangedProbe probe)
    {
        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Services.AddLogging();
        hostBuilder.Services.AddSingleton(probe);

        hostBuilder.Services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<OrderChangedProbe>();

            cfg.UsingRabbitMq((context, rmq) =>
            {
                rmq.Host(rabbitMqConnectionString);
                rmq.ReceiveEndpoint("worker-events-tests-probe", e =>
                {
                    e.ConfigureConsumer<OrderChangedProbe>(context);
                });
            });
        });

        return hostBuilder.Build();
    }

    private sealed class OrderChangedProbe : IConsumer<OrderChanged>
    {
        private readonly Channel<OrderChanged> _messages = Channel.CreateBounded<OrderChanged>(10);

        public async Task<OrderChanged> NextAsync(TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            try
            {
                return await _messages.Reader.ReadAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"OrderChanged не получен за {timeout.TotalSeconds} секунд.");
            }
        }

        public Task Consume(ConsumeContext<OrderChanged> context)
            => _messages.Writer.WriteAsync(context.Message).AsTask();
    }
}
