using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace RentControl.IntegrationTests.Infrastructure;

internal static class MassTransitTestExtensions
{
    /// <summary>
    /// Заставляет MassTransitHostedService ждать полной готовности шины и её receive-endpoint'ов
    /// (подключение к RabbitMQ, объявление exchange/queue и биндингов) до возврата из StartAsync.
    /// Без этого публикация первого сообщения в тесте может уйти в unroutable до завершения биндинга.
    /// </summary>
    public static IServiceCollection WaitMassTransitUntilStarted(this IServiceCollection services)
    {
        services.AddOptions<MassTransitHostOptions>()
            .Configure(options => options.WaitUntilStarted = true);

        return services;
    }
}
