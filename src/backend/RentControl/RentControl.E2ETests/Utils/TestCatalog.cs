using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Main.Model.Models;
using Xunit;

namespace RentControl.E2ETests.Utils;

public sealed record TestCatalog(Guid LocationId, Guid ProductId);

/// <summary>Создаёт каталог (локация, товар, единица в сдающемся состоянии) под админским токеном.</summary>
public static class TestCatalogCreator
{
    public static async Task<TestCatalog> CreateAsync(
        HttpClient mainApi,
        string adminToken,
        CancellationToken cancellationToken)
    {
        var locationId = (await PostAsync(
                mainApi, adminToken, "api/locations",
                new { name = $"e2e-loc-{Guid.NewGuid():N}", address = "e2e-test-address" },
                cancellationToken))
            .GetProperty("id").GetGuid();

        var productId = (await PostAsync(
                mainApi, adminToken, "api/products",
                new
                {
                    name = $"e2e-product-{Guid.NewGuid():N}",
                    marketValue = 1000m,
                    dailyRate = 10m,
                    hourlyRate = 2m,
                },
                cancellationToken))
            .GetProperty("id").GetGuid();

        await PostAsync(
            mainApi, adminToken, "api/productunits",
            new
            {
                productId,
                state = (int)StateEnum.Good,
                serialNumber = $"e2e-sn-{Guid.NewGuid():N}",
                purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow),
                purchasePrice = 500m,
                locationId,
            },
            cancellationToken);

        return new TestCatalog(locationId, productId);
    }

    private static async Task<JsonElement> PostAsync(
        HttpClient mainApi,
        string adminToken,
        string url,
        object body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        request.Content = JsonContent.Create(body);

        using var response = await mainApi.SendAsync(request, cancellationToken);
        Assert.True(response.IsSuccessStatusCode,
            $"{url} не создал объект: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync(cancellationToken)}");

        return await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
    }
}
