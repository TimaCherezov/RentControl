using Main.Api.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Overdue;

public class OverdueBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, IBookingStatusStateService bookingStatusService)
    : IConsumer<OverdueBookingCommand>
{
	public async Task Consume(ConsumeContext<OverdueBookingCommand> context)
	{
		var command = context.Message;

		await uowFactory.ExecuteAsync(async uow =>
		{
			var booking = await uow.Bookings.GetByIdAsync(command.Id, context.CancellationToken);

			if (booking is null)
				throw new ObjectNotFoundException($"Booking with ID: {command.Id} not found.");

			await bookingStatusService.SwitchBookingStatusAsync(
					BookingStatusEnum.Overdue,
					booking,
					context.CancellationToken
			);
		}, context.CancellationToken);
	}
}
