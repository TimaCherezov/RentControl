using Main.Model;
using Main.Model.Models;
using Main.Model.Repositories;
using MassTransit;
using RentControl.Common.IntegrationEvents;

namespace Main.Worker.Consumers;

/// <summary>
/// Счёт оплачен: бронь подтверждается (Pending → Confirmed, что для заказа
/// означает "оплачено") и фронт уведомляется через OrderChanged.
/// Атомарность — Consumer Outbox (см. MainWorkerExtensions).
/// </summary>
public class PaymentSucceededConsumer(
    IBookingRepository bookings,
    MainDbContext dbContext,
    ILogger<PaymentSucceededConsumer> logger) : IConsumer<PaymentSucceeded>
{
    public async Task Consume(ConsumeContext<PaymentSucceeded> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        var booking = await bookings.GetByIdAsync(message.BookingId, ct)
            ?? throw new InvalidOperationException(
                $"Бронь {message.BookingId} не найдена — PaymentSucceeded не может быть обработан, сообщение уйдёт в error-очередь");

        if (booking.BookingStatus == BookingStatusEnum.Confirmed)
        {
            return; // повторная доставка — уже оплачено
        }

        booking.BookingStatus = BookingStatusEnum.Confirmed;
        booking.StatusChangedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(ct);
        await context.Publish(new OrderChanged(booking.Id, booking.BookingStatus.ToString()));

        logger.LogInformation("Бронь {BookingId} подтверждена: счёт оплачен", booking.Id);
    }
}
