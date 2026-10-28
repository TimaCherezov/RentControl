using RentControl.IntegrationTests.Infrastructure;
using Testcontainers.RabbitMq;

namespace RentControl.IntegrationTests.Fixtures;

/// <summary>
/// Реальный RabbitMQ в контейнере — для сервисов, которым нужен брокер
/// (Main.Worker, Payments.Api, Realtime.Api — по мере появления кода).
///
/// Использование: <c>[Collection(nameof(RabbitMqCollection))]</c> и получает
/// фикстуру через конструктор тестового класса. Строку подключения передавайте
/// в конфиг сервиса через <see cref="Infrastructure.IntegrationTestFactory{TProgram}"/>.
/// </summary>
public sealed class RabbitMqFixture : IAsyncLifetime
{
    private RabbitMqContainer _rabbitMq = null!;

    /// <summary>Строка подключения amqp:// к тестовому брокеру.</summary>
    public string ConnectionString => _rabbitMq.GetConnectionString();

    public async Task InitializeAsync()
    {
        _rabbitMq = new RabbitMqBuilder(TestImages.RabbitMq)
            .Build();
        await _rabbitMq.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_rabbitMq is not null)
        {
            await _rabbitMq.DisposeAsync();
        }
    }
}
