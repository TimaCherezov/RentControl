using Main.Api.Exceptions;
using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.Return;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Bookings;

public class ReturnBookingConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();
    private readonly Mock<IBookingStatusStateService> _stateService = new();

    public ReturnBookingConsumerTests()
    {
        _uow.SetupGet(u => u.Bookings).Returns(_bookings.Object);
        _factory.RunsWith(_uow);
    }

    [Fact]
    public async Task Consume_WhenBookingNotFound_ThrowsObjectNotFoundException()
    {
        var bookingId = Guid.NewGuid();
        _bookings
            .Setup(b => b.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new ReturnBookingCommand(bookingId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenBookingExists_SwitchesBookingToReturned()
    {
        var booking = new BookingBuilder()
            .WithStatus(BookingStatusEnum.Active)
            .Build();
        _bookings
            .Setup(b => b.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new ReturnBookingCommand(booking.Id));

        await consumer.Consume(context.Object);

        _stateService.Verify(
            s => s.SwitchBookingStatusAsync(BookingStatusEnum.Returned, It.Is<Booking>(b => b.Id == booking.Id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private ReturnBookingConsumer CreateConsumer() => new(_factory.Object, _stateService.Object);
}
