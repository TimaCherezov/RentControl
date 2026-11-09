using MassTransit;
using Payments.Api.Exceptions;
using Payments.Model.Models;
using Payments.Model.UnitOfWork;

namespace Payments.Api.Features.Invoices.Pay;

public class PayInvoiceConsumer(IPaymentsUnitOfWork uow) : IConsumer<PayInvoiceCommand>
{
    public async Task Consume(ConsumeContext<PayInvoiceCommand> context)
    {
        var ct = context.CancellationToken;

        var invoice = await uow.Invoices.GetByBookingIdAsync(context.Message.BookingId, ct)
            ?? throw new ObjectNotFoundException(
                $"Счёт по брони {context.Message.BookingId} не найден");

        if (invoice.Status != InvoiceStatus.Pending)
            throw new PaymentAlreadyPaidException("Счёт уже оплачен");

        await context.RespondAsync(new PayInvoiceResponse($"/fake-pay/{invoice.Id}"));
    }
}
