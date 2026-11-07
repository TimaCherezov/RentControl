using Main.Model;
using Main.Worker.Consumers;
using MassTransit;

namespace Main.Worker;

public static class MainWorkerExtensions
{
    public static IHostApplicationBuilder AddMainWorkerBus(this IHostApplicationBuilder builder)
    {
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
