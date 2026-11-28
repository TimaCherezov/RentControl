using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RentControl.IntegrationTests.Infrastructure;

/// <summary>
/// Поднимает лёгкий MassTransit-хост (publisher или consumer-пробу) для интеграционных тестов
/// и гарантирует, что шина и её receive-endpoint'ы готовы до возврата из StartAsync.
/// </summary>
internal sealed class MassTransitTestHost : IAsyncDisposable
{
    private readonly IHost _host;

    private MassTransitTestHost(IHost host) => _host = host;

    public IBus Bus => _host.Services.GetRequiredService<IBus>();

    public Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class =>
        Bus.Publish(message, cancellationToken);

    public static Task<MassTransitTestHost> StartPublisherAsync(string rabbitMqConnectionString) =>
        StartAsync(rabbitMqConnectionString, configure: null);

    public static Task<MassTransitTestHost> StartConsumerAsync<TConsumer>(
        string rabbitMqConnectionString,
        string queueName,
        TConsumer consumer) where TConsumer : class, IConsumer =>
        StartAsync(rabbitMqConnectionString, (services, cfg) =>
        {
            services.AddSingleton(consumer);
            cfg.AddConsumer<TConsumer>();
            cfg.UsingRabbitMq((context, rmq) =>
            {
                rmq.Host(rabbitMqConnectionString);
                rmq.ReceiveEndpoint(queueName, e =>
                {
                    e.ConfigureConsumer<TConsumer>(context);
                });
            });
        });

    private static async Task<MassTransitTestHost> StartAsync(
        string rabbitMqConnectionString,
        Action<IServiceCollection, IBusRegistrationConfigurator>? configure)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddLogging();
        builder.Services.WaitMassTransitUntilStarted();

        builder.Services.AddMassTransit(cfg =>
        {
            if (configure is not null)
            {
                configure(builder.Services, cfg);
            }
            else
            {
                cfg.UsingRabbitMq((_, rmq) => rmq.Host(rabbitMqConnectionString));
            }
        });

        var host = builder.Build();
        await host.StartAsync();
        return new MassTransitTestHost(host);
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}

internal static class MassTransitServiceCollectionExtensions
{
    /// <summary>
    /// Заставляет MassTransitHostedService ждать полной готовности шины и её receive-endpoint'ов
    /// до возврата из StartAsync, чтобы первая публикация не уходила в unroutable.
    /// </summary>
    public static IServiceCollection WaitMassTransitUntilStarted(this IServiceCollection services)
    {
        services.AddOptions<MassTransitHostOptions>()
            .Configure(options => options.WaitUntilStarted = true);

        return services;
    }
}
