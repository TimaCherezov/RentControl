using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payments.Model.Models;
using RentControl.Common.IntegrationEvents;
using RentControl.IntegrationTests.Payments.Infrastructure;

namespace RentControl.IntegrationTests.Payments;

[Collection(PaymentsApiCollection.Name)]
public class InvoiceFlowTests(PaymentsApiFixture fixture)
{
    [Fact]
    public async Task OrderCreated_PaymentsCreatesPendingInvoice_AndPublishesInvoiceCreated()
    {
        var bookingId = Guid.NewGuid();
        const decimal amount = 2500m;

        var probe = new InvoiceCreatedProbe();
        var probeHost = StartProbeHost(fixture.RabbitMqConnectionString, probe);
        await probeHost.StartAsync();
        try
        {
            // Act: публикуем OrderCreated от имени Main.Api через пробу-шину.
            var bus = probeHost.Services.GetRequiredService<IBus>();
            await bus.Publish(new OrderCreated(bookingId, amount));

            // Assert 1: Payments опубликовал InvoiceCreated — событие доходит только
            // после коммита счёта, т.к. шло через outbox.
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var received = await probe.Received.WaitAsync(cts.Token);
            Assert.Equal(bookingId, received.BookingId);

            // Assert 2: счёт в paymentsdb — Pending, сумма совпадает.
            await using var db = fixture.CreateDbContext();
            var invoice = await db.Invoices.SingleOrDefaultAsync(i => i.BookingId == bookingId);
            Assert.NotNull(invoice);
            Assert.Equal(amount, invoice.Amount);
            Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        }
        finally
        {
            await probeHost.StopAsync();
            probeHost.Dispose();
        }
    }

    private static IHost StartProbeHost(string rabbitMqConnectionString, InvoiceCreatedProbe probe)
    {
        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Services.AddLogging();
        hostBuilder.Services.AddSingleton(probe);

        hostBuilder.Services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<InvoiceCreatedProbeConsumer>();

            cfg.UsingRabbitMq((context, rmq) =>
            {
                rmq.Host(rabbitMqConnectionString);
                rmq.ReceiveEndpoint("invoice-flow-tests-probe", e =>
                {
                    e.ConfigureConsumer<InvoiceCreatedProbeConsumer>(context);
                });
            });
        });

        return hostBuilder.Build();
    }

    private sealed class InvoiceCreatedProbe : IConsumer<InvoiceCreated>
    {
        private readonly TaskCompletionSource<InvoiceCreated> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<InvoiceCreated> Received => _completion.Task;

        public Task Consume(ConsumeContext<InvoiceCreated> context)
        {
            _completion.TrySetResult(context.Message);
            return Task.CompletedTask;
        }
    }

    private sealed class InvoiceCreatedProbeConsumer(InvoiceCreatedProbe probe) : IConsumer<InvoiceCreated>
    {
        public Task Consume(ConsumeContext<InvoiceCreated> context) => probe.Consume(context);
    }
}
