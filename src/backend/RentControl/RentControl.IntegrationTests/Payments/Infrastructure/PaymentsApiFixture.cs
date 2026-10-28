extern alias PaymentsApi;

using PaymentsApiProgram = PaymentsApi::Program;

using Microsoft.EntityFrameworkCore;
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
    public const string Issuer = "RentControl";
    public const string Audience = "RentControl";

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

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder(TestImages.Postgres)
            .WithDatabase(DatabaseName)
            .Build();
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
                services.AddHttpClient(JwtAuthorizationExtensions.JwksHttpClientName)
                    .ConfigurePrimaryHttpMessageHandler(() => new JwksStubHandler(_jwtKeys));
            });

        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }

        if (_postgres is not null)
        {
            await _postgres.DisposeAsync();
        }

        _jwtKeys.Dispose();
    }

    /// <summary>Контекст БД — для сидирования данных и проверок состояния.</summary>
    public PaymentsDbContext CreateDbContext() =>
        _factory.Services.GetRequiredService<IDbContextFactory<PaymentsDbContext>>().CreateDbContext();

    /// <summary>
    /// Access-токен, который Payments.Api примет: подписан ключом из заглушки JWKS.
    /// </summary>
    public string CreateAccessToken(Guid userId) =>
        _jwtKeys.CreateAccessToken(userId, Issuer, Audience);

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
