using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Main.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Net.Security;
using RentControl.AppHost;
using RentControl.E2ETests.Utils;
using Xunit;

namespace RentControl.E2ETests;

public sealed class AppHostFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public HttpClient MainApi { get; private set; } = null!;

    public HttpClient PaymentsApi { get; private set; } = null!;

    public AdminCredentials Admin { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.RentControl_AppHost>(
                [$"Services={string.Join(',', [ServiceNames.MainApi, ServiceNames.MainWorker, ServiceNames.PaymentsApi])}"],
                (_, settings) => settings.EnvironmentName = Environments.Development);

        Assert.Contains(
            builder.Resources,
            r => r.Name == ServiceNames.MainWorker);
        Assert.DoesNotContain(
            builder.Resources,
            r => r.Name is ServiceNames.RealtimeApi or ServiceNames.Front);

        _app = await builder.BuildAsync();
        await _app.StartAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var cancellationToken = cts.Token;

        await _app.ResourceNotifications.WaitForResourceHealthyAsync(ServiceNames.MainApi, cancellationToken);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync(ServiceNames.MainWorker, cancellationToken);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync(ServiceNames.PaymentsApi, cancellationToken);

        await MigrateMainDatabaseAsync(cancellationToken);

        MainApi = CreateClient(_app, ServiceNames.MainApi);
        PaymentsApi = CreateClient(_app, ServiceNames.PaymentsApi);

        Admin = await AdminSeeder.SeedAsync(this, cancellationToken);
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

    /// <summary>Выполняет действие на прямом подключении к maindb (аранжировка тестовых данных).</summary>
    public async Task<T> WithMainDbAsync<T>(
        Func<MainDbContext, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken)
    {
        var connectionString = await _app!.GetConnectionStringAsync("maindb", cancellationToken)
                               ?? throw new InvalidOperationException("Не удалось получить строку подключения maindb");

        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new MainDbContext(options);
        return await action(context, cancellationToken);
    }

    public async Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken)
    {
        return await TestUsers.GetAccessTokenAsync(
            MainApi, Admin.Email, Admin.Password, cancellationToken);
    }

    private async Task MigrateMainDatabaseAsync(CancellationToken cancellationToken)
    {
        await WithMainDbAsync(async (context, token) =>
        {
            await context.Database.MigrateAsync(token);
            return true;
        }, cancellationToken);
    }

    private static HttpClient CreateClient(DistributedApplication app, string resourceName)
    {
        var handler = new SocketsHttpHandler
        {
            UseCookies = false,
            SslOptions = new SslClientAuthenticationOptions
            {
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