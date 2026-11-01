using Main.Api.Exceptions;
using Main.Api.Features.Bookings.Commands;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Consumers;

public class ReturnBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<ReturnBookingCommand>
{
	public async Task Consume(ConsumeContext<ReturnBookingCommand> context)
	{
		var command = context.Message;

		await uowFactory.ExecuteAsync(async uow =>
		{
			var booking = await uow.Bookings.GetByIdAsync(command.Id, context.CancellationToken);

			if (booking is null)
				throw new ObjectNotFoundException($"Booking with ID: {command.Id} not found.");

			if (booking.BookingStatus is not BookingStatusEnum.Active and not BookingStatusEnum.Overdue)
				throw new InvalidOperationException($"Booking with ID: {command.Id} cannot be returned.");

			var now = DateTimeOffset.UtcNow;

			booking.ActualReturnAt = now;
			booking.BookingStatus = BookingStatusEnum.Returned;
			booking.StatusChangedAt = now;
		}, context.CancellationToken);
	}
}