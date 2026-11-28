using RentControl.ServiceDefaults.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Cancel;

public class CancelBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, IBookingStatusStateService bookingStatusService)
    : IConsumer<CancelBookingCommand>
{
	public async Task Consume(ConsumeContext<CancelBookingCommand> context)
	{
		var command = context.Message;

		await uowFactory.ExecuteAsync(async uow =>
		{
			var booking = await uow.Bookings.GetByIdAsync(command.Id, context.CancellationToken);

			if (booking is null)
				throw new ObjectNotFoundException($"Booking with ID: {command.Id} not found.");
            
            var userId = command.UserId;
            if (userId != booking.UserId)
                throw new ForbiddenException("Access to this booking is denied.");

			await bookingStatusService.SwitchBookingStatusAsync(
					BookingStatusEnum.Cancelled,
					booking,
					context.CancellationToken
			);
		}, context.CancellationToken);
	}
}
