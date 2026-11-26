using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RentControl.E2ETests.Utils;

public static class TestBookings
{
    public static async Task<JsonElement> CreateAsync(
        HttpClient mainApi,
        string accessToken,
        TestCatalog catalog,
        CancellationToken cancellationToken)
    {
        var startAt = DateTimeOffset.UtcNow.AddDays(1);
        var endAt = startAt.AddDays(2);

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/bookings");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(
            new { productId = catalog.ProductId, startAt, endAt, locationId = catalog.LocationId });

        using var response = await mainApi.SendAsync(request, cancellationToken);
        Assert.True(response.IsSuccessStatusCode,
            $"Создание брони не удалось: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync(cancellationToken)}");

        return await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
    }

    public static async Task<JsonElement> GetByIdAsync(
        HttpClient mainApi,
        string accessToken,
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/bookings/{bookingId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await mainApi.SendAsync(request, cancellationToken);
        Assert.True(response.IsSuccessStatusCode,
            $"GET api/bookings/{bookingId} не удался: {(int)response.StatusCode}");

        return await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
    }
}
