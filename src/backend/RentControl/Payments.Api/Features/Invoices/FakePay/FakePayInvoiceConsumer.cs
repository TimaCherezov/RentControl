using MassTransit;
using Payments.Api.Exceptions;
using Payments.Model.Models;
using Payments.Model.UnitOfWork;
using RentControl.Common.IntegrationEvents;

namespace Payments.Api.Features.Invoices.FakePay;

public class FakePayInvoiceConsumer(
    IPaymentsUnitOfWork uow,
    IPublishEndpoint publishEndpoint,
    ILogger<FakePayInvoiceConsumer> logger) : IConsumer<FakePayInvoiceCommand>
{
    public async Task Consume(ConsumeContext<FakePayInvoiceCommand> context)
    {
        var ct = context.CancellationToken;

        await using var transaction = await uow.Context.Database.BeginTransactionAsync(ct);

        var invoice = await uow.Invoices.GetByIdAsync(context.Message.InvoiceId, ct)
            ?? throw new ObjectNotFoundException(
                $"Счёт {context.Message.InvoiceId} не найден");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new PaymentAlreadyPaidException("Счёт уже оплачен — повторная оплата отклонена");

        invoice.Status = InvoiceStatus.Paid;
        await publishEndpoint.Publish(new PaymentSucceeded(invoice.BookingId), ct);
        await uow.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        logger.LogInformation("Счёт {InvoiceId} по брони {BookingId} оплачен", invoice.Id, invoice.BookingId);
        await context.RespondAsync(new FakePayInvoiceResponse(invoice.BookingId));
    }
}
