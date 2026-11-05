using Main.Api.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Return;

public class ReturnBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, IBookingStatusStateService bookingStatusService)
    : IConsumer<ReturnBookingCommand>
{
	public async Task Consume(ConsumeContext<ReturnBookingCommand> context)
	{
		var command = context.Message;

		await uowFactory.ExecuteAsync(async uow =>
		{
			var booking = await uow.Bookings.GetByIdAsync(command.Id, context.CancellationToken);

			if (booking is null)
				throw new ObjectNotFoundException($"Booking with ID: {command.Id} not found.");
            
            await bookingStatusService.SwitchBookingStatusAsync(
                    BookingStatusEnum.Returned, 
                    booking, 
                    context.CancellationToken
            );
		}, context.CancellationToken);
	}
}
