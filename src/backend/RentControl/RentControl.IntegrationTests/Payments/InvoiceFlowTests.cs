using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payments.Model.Models;
using RentControl.Common.IntegrationEvents;
using RentControl.IntegrationTests.Infrastructure;
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
        await using var bus = await MassTransitTestHost.StartConsumerAsync(
            fixture.RabbitMqConnectionString,
            "invoice-flow-tests-probe",
            probe);

        await bus.Publish(new OrderCreated(bookingId, amount));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var received = await probe.Received.WaitAsync(cts.Token);
        Assert.Equal(bookingId, received.BookingId);

        await using var db = fixture.CreateDbContext();
        var invoice = await db.Invoices.SingleOrDefaultAsync(i => i.BookingId == bookingId, cancellationToken: cts.Token);
        Assert.NotNull(invoice);
        Assert.Equal(amount, invoice.Amount);
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
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
}
