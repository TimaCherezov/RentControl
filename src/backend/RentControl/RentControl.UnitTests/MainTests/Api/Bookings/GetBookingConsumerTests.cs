using Main.Api.Exceptions;
using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.Get;
using Main.Api.Features.ProductImages;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Bookings;

public class GetBookingConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetBookingConsumerTests()
    {
        _uow.SetupGet(u => u.Bookings).Returns(_bookings.Object);
        _factory.RunsWith<Booking>(_uow);
    }

    [Fact]
    public async Task Consume_WhenBookingNotFound_ThrowsObjectNotFoundException()
    {
        var bookingId = Guid.NewGuid();
        _bookings
            .Setup(b => b.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetBookingCommand(bookingId, Guid.NewGuid()));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUserIsNotOwner_ThrowsInvalidOperationException()
    {
        var booking = new BookingBuilder().WithUserId(Guid.NewGuid()).Build();
        _bookings
            .Setup(b => b.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetBookingCommand(booking.Id, Guid.NewGuid()));

        await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenUserIsOwner_RespondsWithBookingDto()
    {
        var userId = Guid.NewGuid();
        var booking = new BookingBuilder().WithUserId(userId).Build();
        _bookings
            .Setup(b => b.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetBookingCommand(booking.Id, userId));
        BookingDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<BookingDto>()))
            .Callback<BookingDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(booking.Id, responded.Id);
        Assert.Equal(BookingStatusEnum.Pending, responded.BookingStatus);
    }

    private GetBookingConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }));
}
