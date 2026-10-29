using Main.Api.Exceptions;
using Main.Api.Features.Bookings.Commands;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Consumers;

public class ReturnBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<ReturnBookingCommand>
{
    public async Task Consume(ConsumeContext<ReturnBookingCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var booking = await uow.Bookings.GetByIdAsync(command.Id, context.CancellationToken);
            if (booking is null)
            {
                throw new NotFoundException($"Booking with id {command.Id} not found");
            }

            booking.ActualReturnAt = DateTimeOffset.UtcNow;
            booking.BookingStatusId = BookingStatusEnum.RETURNED;
            booking.StatusChangedAt = DateTimeOffset.UtcNow;

            uow.Bookings.Update(booking);
        }, context.CancellationToken);
    }
}
