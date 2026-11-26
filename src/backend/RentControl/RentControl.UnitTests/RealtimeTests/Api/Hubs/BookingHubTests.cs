using Realtime.Api.Hubs;

namespace RentControl.UnitTests.RealtimeTests.Api.Hubs;

public class BookingHubTests
{
    [Theory]
    [InlineData("01234567-89ab-cdef-0123-456789abcdef", "user:01234567-89ab-cdef-0123-456789abcdef")]
    [InlineData("89abcdef-0123-4567-89ab-cdef01234567", "user:89abcdef-0123-4567-89ab-cdef01234567")]
    public void GetUserGroup_ReturnsUserPrefixedGroupId(string userId, string expectedGroup)
    {
        var group = BookingHub.GetUserGroup(Guid.Parse(userId));

        Assert.Equal(expectedGroup, group);
    }
}
