using Main.Api.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Issue;

public class IssueBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<IssueBookingCommand>
{
	public async Task Consume(ConsumeContext<IssueBookingCommand> context)
	{
		var command = context.Message;

		await uowFactory.ExecuteAsync(async uow =>
		{
			var booking = await uow.Bookings.GetByIdAsync(command.Id, context.CancellationToken);

			if (booking is null)
				throw new ObjectNotFoundException($"Booking with ID: {command.Id} not found.");

			if (booking.BookingStatus != BookingStatusEnum.ReadyForPickup)
				throw new InvalidOperationException($"Booking with ID: {command.Id} is not ready for pickup.");

			booking.BookingStatus = BookingStatusEnum.Active;
			booking.StatusChangedAt = DateTimeOffset.UtcNow;
		}, context.CancellationToken);
	}
}
