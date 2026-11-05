using Mapster;
using MassTransit;
using Payments.Api.Mappers;
using Payments.Model;
using Payments.Model.Models;
using Payments.Model.Repositories;
using RentControl.Common.IntegrationEvents;

namespace Payments.Api.Features.Invoices;

/// <summary>
/// По событию Main "создан заказ" выставляет счёт. Атомарность обеспечивает
/// Consumer Outbox (UseEntityFrameworkOutbox на эндпоинте): запись счёта и
/// публикация InvoiceCreated попадают в одну транзакцию, доставка — после коммита.
/// Репозиторий и контекст здесь scoped: это тот же экземпляр PaymentsDbContext,
/// в котором outbox-мидлварь ведёт транзакцию — иначе атомарность теряется.
/// </summary>
public class OrderCreatedConsumer(
    IInvoiceRepository invoices,
    PaymentsDbContext dbContext,
    ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        // Идемпотентность: 1:1 бронь-счёт (плюс уникальный индекс в БД).
        if (await invoices.ExistsByBookingIdAsync(message.BookingId, ct))
        {
            logger.LogInformation("Счёт по брони {BookingId} уже существует — пропуск", message.BookingId);
            return;
        }

        var invoice = message.Adapt<Invoice>();

        invoices.Add(invoice);
        await dbContext.SaveChangesAsync(ct);
        await context.Publish(new InvoiceCreated(invoice.BookingId));

        logger.LogInformation(
            "Выставлен счёт {InvoiceId} на {Amount} по брони {BookingId}",
            invoice.Id, invoice.Amount, invoice.BookingId);
    }
}
