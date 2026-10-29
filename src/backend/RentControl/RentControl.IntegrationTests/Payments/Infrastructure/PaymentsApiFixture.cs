extern alias PaymentsApi;

using PaymentsApiProgram = PaymentsApi::Program;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Model;
using RentControl.IntegrationTests.Infrastructure;
using RentControl.ServiceDefaults;
using Testcontainers.PostgreSql;

namespace RentControl.IntegrationTests.Payments.Infrastructure;

/// <summary>
/// Payments.Api целиком в памяти (WebApplicationFactory) + реальный Postgres в контейнере.
/// Зависимость от Main.Api (JWKS) мокается на HTTP-границе: клиент "jwt-jwks" получает
/// заглушку с тестовым ключом, токены выпускает <see cref="CreateAccessToken"/>.
/// </summary>
public sealed class PaymentsApiFixture : IAsyncLifetime
{
    private const string DatabaseName = "paymentsdb";

    // Значение не важно: до сети запрос не доходит, обработчик подменён заглушкой.
    private const string MainApiStubUrl = "http://main-api-stub";

    private readonly TestJwtKeys _jwtKeys = new();
    private PostgreSqlContainer _postgres = null!;
    private IntegrationTestFactory<PaymentsApiProgram> _factory = null!;

    /// <summary>Готовый клиент к Payments.Api.</summary>
    public HttpClient Client { get; private set; } = null!;

    /// <summary>Строка подключения к тестовой базе paymentsdb.</summary>
    public string ConnectionString => _postgres.GetConnectionString();

    /// <summary>Issuer из конфигурации сервиса.</summary>
    public string Issuer => GetJwtSetting("Issuer");

    /// <summary>Audience из конфигурации сервиса.</summary>
    public string Audience => GetJwtSetting("Audience");


    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder(TestImages.Postgres)
            .WithDatabase(DatabaseName)
            .Build();
        try
        {
            await _postgres.StartAsync();
            await CreateDatabaseAsync();

            _factory = new IntegrationTestFactory<PaymentsApiProgram>(
                configuration: new Dictionary<string, string?>
                {
                    ["ConnectionStrings:paymentsdb"] = ConnectionString,
                    ["ServiceDiscovery:MainApi"] = MainApiStubUrl,
                },
                configureServices: services =>
                {
                    // Работает за счёт накопления named-конфигураций HttpClientFactory:
                    // последняя подмена primary-обработчика побеждает. Если в сервисе
                    // заведут собственный ConfigurePrimaryHttpMessageHandler для этого
                    // клиента, заглушка перестанет применяться — тесты упадут с
                    // DNS-ошибкой на main-api-stub.
                    services.AddHttpClient(JwtAuthorizationExtensions.JwksHttpClientName)
                        .ConfigurePrimaryHttpMessageHandler(() => new JwksStubHandler(_jwtKeys));
                });

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

            if (_factory is not null)
            {
                await _factory.DisposeAsync();
            }
        }
        finally
        {
            // Контейнер и ключи освобождаем даже при ошибке очистки хоста,
            // чтобы не утечь в параллельные прогоны.
            if (_postgres is not null)
            {
                await _postgres.DisposeAsync();
            }

            _jwtKeys.Dispose();
        }
    }

    /// <summary>Контекст БД — для сидирования данных и проверок состояния. Вызывающий код обязан освободить контекст (await using).</summary>
    public PaymentsDbContext CreateDbContext() =>
        _factory.Services.GetRequiredService<IDbContextFactory<PaymentsDbContext>>().CreateDbContext();

    /// <summary>
    /// Access-токен, который Payments.Api примет: подписан ключом из заглушки JWKS,
    /// issuer/audience — из конфигурации сервиса.
    /// </summary>
    public string CreateAccessToken(Guid userId, TimeSpan? lifetime = null) =>
        _jwtKeys.CreateAccessToken(userId, Issuer, Audience, lifetime);

    private string GetJwtSetting(string key) =>
        _factory.Services.GetRequiredService<IConfiguration>()[$"{JwtOptions.Section}:{key}"]
        ?? throw new InvalidOperationException($"В конфигурации Payments.Api не найден Jwt:{key}");

    // TODO: у Payments.Model пока нет миграций — создаём схему по модели.
    // Когда появятся — заменить на Database.MigrateAsync(), как в MainApiFixture.
    private async Task CreateDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new PaymentsDbContext(options);
        await context.Database.EnsureCreatedAsync();
    }
}
