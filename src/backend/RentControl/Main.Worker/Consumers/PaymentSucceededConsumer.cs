using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.IntegrationEvents;

namespace Main.Worker.Consumers;

public class PaymentSucceededConsumer(
    IMainUnitOfWork uow,
    ILogger<PaymentSucceededConsumer> logger) : IConsumer<PaymentSucceeded>
{
    public async Task Consume(ConsumeContext<PaymentSucceeded> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        var booking = await uow.Bookings.GetByIdAsync(message.BookingId, ct)
            ?? throw new InvalidOperationException(
                $"Бронь {message.BookingId} не найдена — PaymentSucceeded не может быть обработан");

        if (booking.BookingStatus == BookingStatusEnum.Confirmed)
        {
            return; 
        }

        booking.BookingStatus = BookingStatusEnum.Confirmed;
        booking.StatusChangedAt = DateTimeOffset.UtcNow;
        await uow.SaveChangesAsync(ct);
        await context.Publish(new OrderChanged(booking.Id, booking.BookingStatus.ToString()), ct);

        logger.LogInformation("Бронь {BookingId} подтверждена: счёт оплачен", booking.Id);
    }
}
