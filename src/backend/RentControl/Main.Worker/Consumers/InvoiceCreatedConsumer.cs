using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.IntegrationEvents;

namespace Main.Worker.Consumers;

public class InvoiceCreatedConsumer(
    IMainUnitOfWork uow,
    ILogger<InvoiceCreatedConsumer> logger) : IConsumer<InvoiceCreated>
{
    public async Task Consume(ConsumeContext<InvoiceCreated> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        var booking = await uow.Bookings.GetByIdAsync(message.BookingId, ct)
            ?? throw new InvalidOperationException(
                $"Бронь {message.BookingId} не найдена — InvoiceCreated не может быть обработан");

        if (booking.IsInvoiceCreated)
        {
            return; 
        }

        booking.IsInvoiceCreated = true;
        await uow.SaveChangesAsync(ct);
        await context.Publish(new OrderChanged(booking.Id, booking.BookingStatus.ToString()), ct);

        logger.LogInformation("Бронь {BookingId} готова к оплате: счёт выставлен", booking.Id);
    }
}
