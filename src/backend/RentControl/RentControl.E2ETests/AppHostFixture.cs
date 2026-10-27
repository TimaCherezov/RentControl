using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Main.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Net.Security;
using System.Security.Authentication.ExtendedProtection;
using Xunit;

namespace RentControl.E2ETests;

public sealed class AppHostFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public HttpClient MainApi { get; private set; } = null!;

    public HttpClient PaymentsApi { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.RentControl_AppHost>(
                [$"Services={string.Join(',', [ServiceNames.MainApi, ServiceNames.PaymentsApi])}"],
                (_, settings) => settings.EnvironmentName = Environments.Development);

        Assert.DoesNotContain(
            builder.Resources,
            r => r.Name is ServiceNames.MainWorker or ServiceNames.RealtimeApi);

        _app = await builder.BuildAsync();
        await _app.StartAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var cancellationToken = cts.Token;

        await _app.ResourceNotifications.WaitForResourceHealthyAsync(ServiceNames.MainApi, cancellationToken);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync(ServiceNames.PaymentsApi, cancellationToken);
        
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