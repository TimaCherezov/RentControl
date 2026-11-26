using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using Main.Worker.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using RentControl.Common.IntegrationEvents;

namespace RentControl.UnitTests.MainTests.Worker;

public class PaymentSucceededConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IBookingRepository> _bookings = new();

    public PaymentSucceededConsumerTests()
    {
        _uow.SetupGet(u => u.Bookings).Returns(_bookings.Object);
    }

    [Fact]
    public async Task Consume_WhenBookingNotFound_ThrowsInvalidOperationException()
    {
        var bookingId = Guid.NewGuid();
        _bookings
            .Setup(b => b.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new PaymentSucceeded(bookingId));

        await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenBookingAlreadyConfirmed_DoesNotSaveOrPublish()
    {
        var booking = new BookingBuilder()
            .WithStatus(BookingStatusEnum.Confirmed)
            .Build();
        _bookings
            .Setup(b => b.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new PaymentSucceeded(booking.Id));
        context
            .Setup(p => p.Publish(It.IsAny<OrderChanged>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        context.Verify(
            p => p.Publish(It.IsAny<OrderChanged>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WhenBookingIsPending_SetsConfirmedSavesAndPublishesOrderChanged()
    {
        var booking = new BookingBuilder().Build();
        _bookings
            .Setup(b => b.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new PaymentSucceeded(booking.Id));
        OrderChanged? published = null;
        context
            .Setup(p => p.Publish(It.IsAny<OrderChanged>(), It.IsAny<CancellationToken>()))
            .Callback<OrderChanged, CancellationToken>((message, _) => published = message)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.Equal(BookingStatusEnum.Confirmed, booking.BookingStatus);
        Assert.NotEqual(default, booking.StatusChangedAt);
        Assert.Equal("Confirmed", published?.Status);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private PaymentSucceededConsumer CreateConsumer() =>
        new(_uow.Object, Mock.Of<ILogger<PaymentSucceededConsumer>>());
}
