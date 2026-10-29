using Main.Api.Exceptions;
using Main.Api.Features.Bookings.Commands;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Consumers;

public class IssueBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<IssueBookingCommand>
{
    public async Task Consume(ConsumeContext<IssueBookingCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var booking = await uow.Bookings.GetByIdAsync(command.Id, context.CancellationToken);
            if (booking is null)
            {
                throw new NotFoundException($"Booking with id {command.Id} not found");
            }

            if (booking.BookingStatusId != BookingStatusEnum.READY_FOR_PICKUP)
            {
                throw new InvalidOperationException(
                    $"Booking {command.Id} is not ready for pickup. Current status: {booking.BookingStatus?.Code ?? "Unknown"}");
            }

            booking.BookingStatusId = BookingStatusEnum.ACTIVE;
            booking.StatusChangedAt = DateTimeOffset.UtcNow;

            uow.Bookings.Update(booking);
        }, context.CancellationToken);
    }
}
