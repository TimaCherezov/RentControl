using System.Net;
using System.Net.Http.Json;
using RentControl.IntegrationTests.Realtime.Infrastructure;

namespace RentControl.IntegrationTests.Realtime;

[Collection(RealtimeApiCollection.Name)]
public class RealtimeApiSmokeTests(RealtimeApiFixture fixture)
{
    [Fact]
    public async Task WeatherForecast_IsAnonymous()
    {
        var response = await fixture.Client.GetAsync("/weatherforecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
