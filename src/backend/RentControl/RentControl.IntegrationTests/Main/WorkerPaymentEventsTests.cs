using System.Threading.Channels;
using Main.Model;
using Main.Model.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RentControl.Common.IntegrationEvents;
using RentControl.IntegrationTests.Infrastructure;
using RentControl.IntegrationTests.Main.Infrastructure;

namespace RentControl.IntegrationTests.Main;

[Collection(MainApiCollection.Name)]
public class WorkerPaymentEventsTests(MainApiFixture fixture)
{
    [Fact]
    public async Task InvoiceCreated_ThenPaymentSucceeded_UpdatesBooking_AndPublishesOrderChanged()
    {
        var (bookingId, userId) = await SeedBookingAsync();

        var probe = new OrderChangedProbe();
        await using var bus = await MassTransitTestHost.StartConsumerAsync(
            fixture.RabbitMqConnectionString,
            "worker-events-tests-probe",
            probe);

        await bus.Publish(new InvoiceCreated(bookingId));
        var first = await probe.NextAsync(TimeSpan.FromSeconds(30));
        Assert.Equal(bookingId, first.BookingId);
        Assert.Equal(userId, first.UserId);
        Assert.Equal(nameof(BookingStatusEnum.Pending), first.Status);

        await using (var db = fixture.CreateDbContext())
        {
            var booking = await db.Bookings.AsNoTracking().SingleAsync(b => b.Id == bookingId);
            Assert.True(booking.IsInvoiceCreated);
        }

        await bus.Publish(new PaymentSucceeded(bookingId));
        var second = await probe.NextAsync(TimeSpan.FromSeconds(30));
        Assert.Equal(bookingId, second.BookingId);
        Assert.Equal(userId, second.UserId);
        Assert.Equal(nameof(BookingStatusEnum.Confirmed), second.Status);

        await using (var db = fixture.CreateDbContext())
        {
            var booking = await db.Bookings.AsNoTracking().SingleAsync(b => b.Id == bookingId);
            Assert.Equal(BookingStatusEnum.Confirmed, booking.BookingStatus);
        }
    }

    private async Task<(Guid BookingId, Guid UserId)> SeedBookingAsync()
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

        return (booking.Id, user.Id);
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
