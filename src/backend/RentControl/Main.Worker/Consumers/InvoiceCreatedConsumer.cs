using Main.Model;
using Main.Model.Models;
using Main.Model.Repositories;
using MassTransit;
using RentControl.Common.IntegrationEvents;

namespace Main.Worker.Consumers;

/// <summary>
/// Payments выставил счёт по брони: отмечаем бронь готовой к оплате
/// (фронт по OrderChanged покажет кнопку оплаты) и уведомляем фронт.
/// Атомарность — Consumer Outbox (см. MainWorkerExtensions).
/// </summary>
public class InvoiceCreatedConsumer(
    IBookingRepository bookings,
    MainDbContext dbContext,
    ILogger<InvoiceCreatedConsumer> logger) : IConsumer<InvoiceCreated>
{
    public async Task Consume(ConsumeContext<InvoiceCreated> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        var booking = await bookings.GetByIdAsync(message.BookingId, ct)
            ?? throw new InvalidOperationException(
                $"Бронь {message.BookingId} не найдена — InvoiceCreated не может быть обработан, сообщение уйдёт в error-очередь");

        if (booking.IsInvoiceCreated)
        {
            return; // повторная доставка — уже обработано
        }

        booking.IsInvoiceCreated = true;
        await dbContext.SaveChangesAsync(ct);
        await context.Publish(new OrderChanged(booking.Id, booking.BookingStatus.ToString()));

        logger.LogInformation("Бронь {BookingId} готова к оплате: счёт выставлен", booking.Id);
    }
}
