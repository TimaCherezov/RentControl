extern alias RealtimeApi;

using RealtimeApiProgram = RealtimeApi::Program;

using RentControl.IntegrationTests.Infrastructure;

namespace RentControl.IntegrationTests.Realtime.Infrastructure;

/// <summary>
/// Realtime.Api целиком в памяти (WebApplicationFactory). Внешних зависимостей у
/// сервиса пока нет; когда появится, например, RabbitMQ — контейнер стартует
/// в этой фикстуре (образ: TestImages.RabbitMq).
/// </summary>
public sealed class RealtimeApiFixture : IAsyncLifetime
{
    private IntegrationTestFactory<RealtimeApiProgram> _factory = null!;

    /// <summary>Готовый клиент к Realtime.Api.</summary>
    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _factory = new IntegrationTestFactory<RealtimeApiProgram>();
        try
        {
            Client = _factory.CreateClient();
        }
        catch
        {
            // xUnit не вызывает DisposeAsync, если InitializeAsync упал — чистим сами.
            await DisposeAsync();
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            Client?.Dispose();
        }
        finally
        {
            // Хост освобождаем даже при ошибке очистки клиента — ровно один раз.
            if (_factory is not null)
            {
                await _factory.DisposeAsync();
            }
        }
    }
}
