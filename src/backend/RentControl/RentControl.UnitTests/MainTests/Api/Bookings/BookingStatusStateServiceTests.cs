using Main.Api.Features.Bookings;
using RentControl.ServiceDefaults.Exceptions;
using Main.Model.Models;

namespace RentControl.UnitTests.MainTests.Api.Bookings;

public class BookingStatusStateServiceTests
{
    private readonly BookingStatusStateService _service = new();

    [Theory]
    [InlineData(BookingStatusEnum.Pending, BookingStatusEnum.Confirmed)]
    [InlineData(BookingStatusEnum.Pending, BookingStatusEnum.Cancelled)]
    [InlineData(BookingStatusEnum.Confirmed, BookingStatusEnum.ReadyForPickup)]
    [InlineData(BookingStatusEnum.ReadyForPickup, BookingStatusEnum.Active)]
    [InlineData(BookingStatusEnum.Active, BookingStatusEnum.Returned)]
    [InlineData(BookingStatusEnum.Overdue, BookingStatusEnum.Returned)]
    [InlineData(BookingStatusEnum.Active, BookingStatusEnum.Overdue)]
    [InlineData(BookingStatusEnum.Returned, BookingStatusEnum.Completed)]
    public async Task SwitchBookingStatus_WhenAllowedTransition_SetsTargetStatus(
        BookingStatusEnum from,
        BookingStatusEnum to)
    {
        var booking = new BookingBuilder().WithStatus(from).Build();

        var result = await _service.SwitchBookingStatusAsync(to, booking, CancellationToken.None);

        Assert.Equal(to, result.BookingStatus);
    }

    [Fact]
    public async Task SwitchBookingStatus_WhenReturned_SetsActualReturnAt()
    {
        var booking = new BookingBuilder()
            .WithStatus(BookingStatusEnum.Active)
            .Build();

        await _service.SwitchBookingStatusAsync(BookingStatusEnum.Returned, booking, CancellationToken.None);

        Assert.NotNull(booking.ActualReturnAt);
    }

    [Fact]
    public async Task SwitchBookingStatus_WhenReturnedFromOverdue_SetsActualReturnAt()
    {
        var booking = new BookingBuilder()
            .WithStatus(BookingStatusEnum.Overdue)
            .Build();

        await _service.SwitchBookingStatusAsync(BookingStatusEnum.Returned, booking, CancellationToken.None);

        Assert.NotNull(booking.ActualReturnAt);
    }

    [Fact]
    public async Task SwitchBookingStatus_WhenTransitionAllowed_SetsStatusChangedAt()
    {
        var booking = new BookingBuilder()
            .WithStatus(BookingStatusEnum.Pending)
            .Build();

        await _service.SwitchBookingStatusAsync(BookingStatusEnum.Confirmed, booking, CancellationToken.None);

        Assert.NotEqual(default, booking.StatusChangedAt);
    }

    [Theory]
    [InlineData(BookingStatusEnum.Confirmed, BookingStatusEnum.Cancelled)]
    [InlineData(BookingStatusEnum.Active, BookingStatusEnum.Cancelled)]
    [InlineData(BookingStatusEnum.Pending, BookingStatusEnum.Active)]
    [InlineData(BookingStatusEnum.Returned, BookingStatusEnum.Active)]
    [InlineData(BookingStatusEnum.Cancelled, BookingStatusEnum.Confirmed)]
    public async Task SwitchBookingStatus_WhenIllegalTransition_ThrowsUnprocessableEntityException(
        BookingStatusEnum from,
        BookingStatusEnum to)
    {
        var booking = new BookingBuilder().WithStatus(from).Build();

        await Assert.ThrowsAsync<UnprocessableEntityException>(
            () => _service.SwitchBookingStatusAsync(to, booking, CancellationToken.None));
    }

    [Fact]
    public async Task SwitchBookingStatus_WhenTargetPending_ThrowsUnprocessableEntityException()
    {
        var booking = new BookingBuilder()
            .WithStatus(BookingStatusEnum.Pending)
            .Build();

        await Assert.ThrowsAsync<UnprocessableEntityException>(
            () => _service.SwitchBookingStatusAsync(BookingStatusEnum.Pending, booking, CancellationToken.None));
    }
}
