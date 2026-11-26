using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

public class RefreshTokenTests
{
    [Fact]
    public void IsActive_WhenNotRevokedAndNotExpired_ReturnsTrue()
    {
        var token = new RefreshTokenBuilder().Build();

        Assert.True(token.IsActive);
    }

    [Fact]
    public void IsActive_WhenRevoked_ReturnsFalse()
    {
        var token = new RefreshTokenBuilder().Revoked().Build();

        Assert.False(token.IsActive);
    }

    [Fact]
    public void IsActive_WhenExpired_ReturnsFalse()
    {
        var token = new RefreshTokenBuilder().Expired().Build();

        Assert.False(token.IsActive);
    }
}
