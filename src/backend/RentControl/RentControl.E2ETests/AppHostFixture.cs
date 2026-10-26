using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Main.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Net.Security;
using Xunit;

namespace RentControl.E2ETests;

/// <summary>
/// Поднимает AppHost один раз на тестовый прогон.
/// Фильтр Services=main-api,payments-api оставляет только то,
/// что нужно для кросс-сервисного сценария авторизации:
/// postgres, rabbitmq, main-api, payments-api.
/// </summary>
public sealed class AppHostFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public HttpClient MainApi { get; private set; } = null!;

    public HttpClient PaymentsApi { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        // AppHost читает Services из конфигурации при старте (Program.cs выполняется
        // внутри CreateAsync), поэтому передаём фильтр через переменное окружение.
        var previous = Environment.GetEnvironmentVariable("Services");
        Environment.SetEnvironmentVariable("Services", "main-api,payments-api");
        try
        {
            var builder = await DistributedApplicationTestingBuilder
                .CreateAsync<Projects.RentControl_AppHost>(
                    [],
                    (_, settings) => settings.EnvironmentName = Environments.Development);

            _app = await builder.BuildAsync();
            await _app.StartAsync();
        }
        finally
        {
            Environment.SetEnvironmentVariable("Services", previous);
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var cancellationToken = cts.Token;

        await _app.ResourceNotifications.WaitForResourceHealthyAsync(
            ServiceNames.MainApi, cancellationToken);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync(
            ServiceNames.PaymentsApi, cancellationToken);

        // Тестовый прогон стартует чистую базу (Aspire в тестовом режиме не переиспользует
        // персистентный том из dev), поэтому схему maindb надо накатить вручную.
        await MigrateMainDatabaseAsync(cancellationToken);

        MainApi = CreateClient(_app, ServiceNames.MainApi);
        PaymentsApi = CreateClient(_app, ServiceNames.PaymentsApi);
    }

    public async ValueTask DisposeAsync()
    {
        MainApi?.Dispose();
        PaymentsApi?.Dispose();

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }

    private async Task MigrateMainDatabaseAsync(CancellationToken cancellationToken)
    {
        var connectionString = await _app!.GetConnectionStringAsync("maindb", cancellationToken)
            ?? throw new InvalidOperationException("Не удалось получить строку подключения maindb");

        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new MainDbContext(options);
        await context.Database.MigrateAsync(cancellationToken);
    }

    private static HttpClient CreateClient(DistributedApplication app, string resourceName)
    {
        var handler = new SocketsHttpHandler
        {
            // Куки управляем вручную: так Set-Cookie остаётся в заголовках ответа.
            UseCookies = false,
            SslOptions = new SslClientAuthenticationOptions
            {
                // Dev-сертификат Aspire не доверенный в CI — для e2e это не важно.
                RemoteCertificateValidationCallback = (_, _, _, _) => true
            }
        };

        return new HttpClient(handler)
        {
            BaseAddress = app.GetEndpoint(resourceName, "https"),
            Timeout = TimeSpan.FromMinutes(1)
        };
    }
}

[CollectionDefinition(Name)]
public sealed class AppHostCollection : ICollectionFixture<AppHostFixture>
{
    public const string Name = "AppHost";
}
