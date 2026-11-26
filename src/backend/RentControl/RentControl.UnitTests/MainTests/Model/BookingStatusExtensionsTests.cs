using Main.Model.Models;

namespace RentControl.UnitTests.MainTests.Model;

public class BookingStatusExtensionsTests
{
    [Theory]
    [InlineData(BookingStatusEnum.Pending)]
    [InlineData(BookingStatusEnum.Confirmed)]
    [InlineData(BookingStatusEnum.ReadyForPickup)]
    [InlineData(BookingStatusEnum.Active)]
    [InlineData(BookingStatusEnum.Overdue)]
    public void IsBlocking_WhenStatusIsBlocking_ReturnsTrue(BookingStatusEnum status)
    {
        var result = BookingStatusExtensions.IsBlocking(status);

        Assert.True(result);
    }

    [Theory]
    [InlineData(BookingStatusEnum.Cancelled)]
    [InlineData(BookingStatusEnum.Returned)]
    [InlineData(BookingStatusEnum.Completed)]
    public void IsBlocking_WhenStatusIsNotBlocking_ReturnsFalse(BookingStatusEnum status)
    {
        var result = BookingStatusExtensions.IsBlocking(status);

        Assert.False(result);
    }
}
