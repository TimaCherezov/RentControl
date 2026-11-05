using Main.Model;
using Main.Model.Repositories;
using Main.Worker.Consumers;
using MassTransit;

namespace Main.Worker;

public static class MainWorkerExtensions
{
    public static IHostApplicationBuilder AddMainWorkerBus(this IHostApplicationBuilder builder)
    {
        // Репозитории на scoped-контексте: тот же экземпляр MainDbContext, в котором
        // consumer outbox ведёт транзакцию. Регистрируются вместе с шиной, чтобы
        // хост воркера (в т.ч. тестовый) получал их одним вызовом AddMainWorkerBus.
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();

        builder.Services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<InvoiceCreatedConsumer>();
            cfg.AddConsumer<PaymentSucceededConsumer>();
            cfg.AddEntityFrameworkOutbox<MainDbContext>(outbox =>
            {
                outbox.UsePostgres();
                outbox.QueryDelay = TimeSpan.FromSeconds(1);
            });

            cfg.AddConfigureEndpointsCallback((context, _, endpoint) =>
            {
                endpoint.UseEntityFrameworkOutbox<MainDbContext>(context);
            });

            cfg.UsingRabbitMq((context, rmq) =>
            {
                rmq.Host(builder.Configuration.GetConnectionString("rabbitmq"));
                rmq.ConfigureEndpoints(context);
            });
        });

        return builder;
    }
}
