extern alias RealtimeApi;

using RealtimeApiProgram = RealtimeApi::Program;

using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Realtime.Api.Hubs;
using RentControl.IntegrationTests.Infrastructure;
using Testcontainers.RabbitMq;
using System.Threading.Channels;

namespace RentControl.IntegrationTests.Realtime.Infrastructure;

public sealed class RealtimeApiFixture : IAsyncLifetime
{
    private const string MainApiStubUrl = "http://main-api-stub";

    private RabbitMqContainer _rabbit = null!;
    private IntegrationTestFactory<RealtimeApiProgram> _factory = null!;
    private readonly BookingNotificationProbe _notificationProbe = new();

    public string RabbitMqConnectionString => _rabbit.GetConnectionString();
    public BookingNotificationProbe NotificationProbe => _notificationProbe;

    public HttpClient? Client { get; private set; }

    public async Task InitializeAsync()
    {
        _rabbit = new RabbitMqBuilder(TestImages.RabbitMq)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server startup complete"))
            .Build();
        try
        {
            await _rabbit.StartAsync();

            _factory = new IntegrationTestFactory<RealtimeApiProgram>(
                configuration: new Dictionary<string, string?>
                {
                    ["ConnectionStrings:rabbitmq"] = RabbitMqConnectionString,
                    ["ServiceDiscovery:MainApi"] = MainApiStubUrl,
                    ["Jwt:Issuer"] = "RentControl",
                    ["Jwt:Audience"] = "RentControl",
                },
                configureServices: services =>
                {
                    services.AddSingleton<IHubContext<BookingHub>>(
                        new BookingHubContext(_notificationProbe));
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
        }
        finally
        {
            // Хост освобождаем даже при ошибке очистки клиента — ровно один раз.
            if (_factory is not null)
            {
                await _factory.DisposeAsync();
            }

            if (_rabbit is not null)
            {
                await _rabbit.DisposeAsync();
            }
        }
    }
}

public record BookingNotification(
    string Group,
    string Method,
    object? Payload);

public class BookingNotificationProbe
{
    private readonly Channel<BookingNotification> _notifications =
        Channel.CreateBounded<BookingNotification>(10);

    public ChannelReader<BookingNotification> Reader => _notifications.Reader;

    public Task SendAsync(
        string group,
        string method,
        object? payload)
        => _notifications.Writer.WriteAsync(
            new BookingNotification(group, method, payload)).AsTask();
}

internal class BookingHubContext(
    BookingNotificationProbe probe) : IHubContext<BookingHub>
{
    public IHubClients Clients { get; } =
        new BookingHubClients(probe);

    public IGroupManager Groups { get; } =
        new BookingHubGroupManager();
}

internal class BookingHubClients(
    BookingNotificationProbe probe) : IHubClients
{
    public IClientProxy All => new BookingClientProxy(probe, "all");

    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds)
        => All;

    public IClientProxy Client(string connectionId)
        => new BookingClientProxy(probe, $"connection:{connectionId}");

    public IClientProxy Clients(IReadOnlyList<string> connectionIds)
        => All;

    public IClientProxy Group(string groupName)
        => new BookingClientProxy(probe, $"group:{groupName}");

    public IClientProxy GroupExcept(
        string groupName,
        IReadOnlyList<string> excludedConnectionIds)
        => Group(groupName);

    public IClientProxy Groups(IReadOnlyList<string> groupNames)
        => All;

    public IClientProxy User(string userId)
        => new BookingClientProxy(probe, $"user:{userId}");

    public IClientProxy Users(IReadOnlyList<string> userIds)
        => All;
}

internal sealed class BookingClientProxy(
    BookingNotificationProbe probe,
    string target) : IClientProxy
{
    public Task SendCoreAsync(
        string method,
        object?[] args,
        CancellationToken cancellationToken = default)
    {
        var payload = args.Length > 0 ? args[0] : null;
        return probe.SendAsync(target, method, payload);
    }
}

internal sealed class BookingHubGroupManager : IGroupManager
{
    public Task AddToGroupAsync(
        string connectionId,
        string groupName,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task RemoveFromGroupAsync(
        string connectionId,
        string groupName,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
