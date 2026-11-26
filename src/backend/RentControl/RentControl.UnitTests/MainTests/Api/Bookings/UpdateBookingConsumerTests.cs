using Main.Api.Exceptions;
using Main.Api.Features.Bookings.Update;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Bookings;

public class UpdateBookingConsumerTests
{
    private static readonly DateTimeOffset UpdatedStartAt = new(2026, 8, 1, 9, 0, 0, TimeSpan.Zero);

    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public UpdateBookingConsumerTests()
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
        var context = TestConsumeContext.For(CreateCommand(bookingId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenBookingExists_AppliesCommandToBooking()
    {
        var booking = new BookingBuilder().Build();
        _bookings
            .Setup(b => b.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(CreateCommand(booking.Id));

        await consumer.Consume(context.Object);

        Assert.Equal(UpdatedStartAt, booking.StartAt);
        Assert.Equal(55m, booking.DepositAmount);
        Assert.Equal(BookingStatusEnum.Active, booking.BookingStatus);
    }

    private UpdateBookingConsumer CreateConsumer() => new(_factory.Object);

    private static UpdateBookingCommand CreateCommand(Guid bookingId) => new(
        bookingId,
        Guid.NewGuid(),
        Guid.NewGuid(),
        UpdatedStartAt,
        UpdatedStartAt.AddDays(2),
        ActualReturnAt: null,
        DepositAmount: 55m,
        TotalRentalCost: 77m,
        BookingStatus: BookingStatusEnum.Active,
        StatusChangedAt: UpdatedStartAt);
}
