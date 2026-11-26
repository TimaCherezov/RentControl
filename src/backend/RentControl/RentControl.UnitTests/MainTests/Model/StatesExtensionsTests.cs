using Main.Model.Models;

namespace RentControl.UnitTests.MainTests.Model;

public class StatesExtensionsTests
{
    [Theory]
    [InlineData(StateEnum.New)]
    [InlineData(StateEnum.Excellent)]
    [InlineData(StateEnum.Good)]
    [InlineData(StateEnum.Used)]
    public void IsRentable_WhenStateIsRentable_ReturnsTrue(StateEnum state)
    {
        var result = StatesExtensions.IsRentable(state);

        Assert.True(result);
    }

    [Theory]
    [InlineData(StateEnum.NeedsMaintenance)]
    [InlineData(StateEnum.NeedsRepair)]
    [InlineData(StateEnum.Broken)]
    [InlineData(StateEnum.WriteOff)]
    public void IsRentable_WhenStateIsNotRentable_ReturnsFalse(StateEnum state)
    {
        var result = StatesExtensions.IsRentable(state);

        Assert.False(result);
    }
}
