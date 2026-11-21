using System.Net;
using RentControl.IntegrationTests.Realtime.Infrastructure;

namespace RentControl.IntegrationTests.Realtime;

[Collection(RealtimeApiCollection.Name)]
public class RealtimeApiSmokeTests(RealtimeApiFixture fixture)
{
    [Fact]
    public async Task BookingHub_ReturnsUnauthorizedWithoutToken()
    {
        var response = await fixture.Client.PostAsync(
            "/hubs/bookings/negotiate",
            content: null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
