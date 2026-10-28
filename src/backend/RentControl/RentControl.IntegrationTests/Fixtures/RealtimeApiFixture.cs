extern alias RealtimeApi;

using RealtimeApiProgram = RealtimeApi::Program;

using RentControl.IntegrationTests.Infrastructure;

namespace RentControl.IntegrationTests.Fixtures;

/// <summary>
/// Realtime.Api целиком в памяти (WebApplicationFactory). Внешних зависимостей у
/// сервиса пока нет; когда появится RabbitMQ — добавьте в фикстуру контейнер
/// (см. <see cref="RabbitMqFixture"/>) и пробросьте строку подключения в конфиг.
///
/// Использование: <c>[Collection(RealtimeApiCollection.Name)]</c> — фикстура общая
/// на коллекцию; либо <c>: IClassFixture&lt;RealtimeApiFixture&gt;</c> — своя на класс.
/// </summary>
public sealed class RealtimeApiFixture : IAsyncLifetime
{
    private IntegrationTestFactory<RealtimeApiProgram> _factory = null!;

    /// <summary>Готовый клиент к Realtime.Api.</summary>
    public HttpClient Client { get; private set; } = null!;

    public Task InitializeAsync()
    {
        _factory = new IntegrationTestFactory<RealtimeApiProgram>();
        Client = _factory.CreateClient();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }
}
