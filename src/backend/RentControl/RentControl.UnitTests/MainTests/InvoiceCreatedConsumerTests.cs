using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using Main.Worker.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using RentControl.Common.IntegrationEvents;

namespace RentControl.UnitTests.MainTests;

public class InvoiceCreatedConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IBookingRepository> _bookings = new();

    public InvoiceCreatedConsumerTests()
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
        var context = TestConsumeContext.For(new InvoiceCreated(bookingId));

        await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenInvoiceAlreadyCreated_DoesNotSaveOrPublish()
    {
        var booking = new BookingBuilder().WithInvoiceCreated().Build();
        _bookings
            .Setup(b => b.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new InvoiceCreated(booking.Id));
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
    public async Task Consume_WhenInvoiceNotCreatedYet_SetsFlagSavesAndPublishesOrderChanged()
    {
        var booking = new BookingBuilder().Build();
        _bookings
            .Setup(b => b.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new InvoiceCreated(booking.Id));
        OrderChanged? published = null;
        context
            .Setup(p => p.Publish(It.IsAny<OrderChanged>(), It.IsAny<CancellationToken>()))
            .Callback<OrderChanged, CancellationToken>((message, _) => published = message)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.True(booking.IsInvoiceCreated);
        Assert.Equal("Pending", published?.Status);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private InvoiceCreatedConsumer CreateConsumer() =>
        new(_uow.Object, Mock.Of<ILogger<InvoiceCreatedConsumer>>());
}
