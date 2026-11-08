using Mapster;
using MassTransit;
using Payments.Model.Models;
using Payments.Model.UnitOfWork;
using RentControl.Common.IntegrationEvents;

namespace Payments.Api.Features.Invoices;

public class OrderCreatedConsumer(
    IPaymentsUnitOfWork uow,
    ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        if (await uow.Invoices.ExistsByBookingIdAsync(message.BookingId, ct))
        {
            logger.LogInformation("Счёт по брони {BookingId} уже существует — пропуск", message.BookingId);
            return;
        }

        var invoice = message.Adapt<Invoice>();

        uow.Invoices.Add(invoice);
        await uow.SaveChangesAsync(ct);
        await context.Publish(new InvoiceCreated(invoice.BookingId), ct);

        logger.LogInformation(
            "Выставлен счёт {InvoiceId} на {Amount} по брони {BookingId}",
            invoice.Id, invoice.Amount, invoice.BookingId);
    }
}
