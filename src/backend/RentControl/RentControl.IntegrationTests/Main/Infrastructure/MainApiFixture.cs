extern alias MainApi;

using MainApiProgram = MainApi::Program;

using Main.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentControl.IntegrationTests.Infrastructure;
using RentControl.ServiceDefaults;
using Testcontainers.PostgreSql;

namespace RentControl.IntegrationTests.Main.Infrastructure;

/// <summary>
/// Main.Api целиком в памяти (WebApplicationFactory) + реальный Postgres в контейнере.
/// Миграции Main.Model накатываются при старте фикстуры.
/// </summary>
public sealed class MainApiFixture : IAsyncLifetime
{
    private const string DatabaseName = "maindb";

    private readonly TestJwtKeys _jwtKeys = new();
    private PostgreSqlContainer _postgres = null!;
    private IntegrationTestFactory<MainApiProgram> _factory = null!;

    /// <summary>Готовый клиент к Main.Api.</summary>
    public HttpClient Client { get; private set; } = null!;

    /// <summary>Строка подключения к тестовой базе maindb.</summary>
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
            await MigrateAsync();

            _factory = new IntegrationTestFactory<MainApiProgram>(new Dictionary<string, string?>
            {
                ["ConnectionStrings:maindb"] = ConnectionString,
                ["Jwt:PrivateKeyPem"] = _jwtKeys.PrivateKeyPem,
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

    /// <summary>Контекст БД — для сидирования данных и проверок состояния. Вызывающий код обязан освободить контекст (await using).</summary>
    public MainDbContext CreateDbContext() =>
        _factory.Services.GetRequiredService<IDbContextFactory<MainDbContext>>().CreateDbContext();

    /// <summary>
    /// Access-токен, подписанный тем же ключом, что использует сервис,
    /// issuer/audience — из конфигурации сервиса.
    /// </summary>
    public string CreateAccessToken(Guid userId, TimeSpan? lifetime = null) =>
        _jwtKeys.CreateAccessToken(userId, Issuer, Audience, lifetime);

    private string GetJwtSetting(string key) =>
        _factory.Services.GetRequiredService<IConfiguration>()[$"{JwtOptions.Section}:{key}"]
        ?? throw new InvalidOperationException($"В конфигурации Main.Api не найден Jwt:{key}");

    private async Task MigrateAsync()
    {
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new MainDbContext(options);
        await context.Database.MigrateAsync();
    }
}
