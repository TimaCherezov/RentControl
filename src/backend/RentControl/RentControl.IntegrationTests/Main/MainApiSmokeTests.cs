using System.Net;
using System.Net.Http.Json;
using RentControl.IntegrationTests.Main.Infrastructure;

namespace RentControl.IntegrationTests.Main;

[Collection(MainApiCollection.Name)]
public class MainApiSmokeTests(MainApiFixture fixture)
{
    [Fact]
    public async Task Alive_ReturnsHealthy()
    {
        var response = await fixture.Client.GetAsync("/alive");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
