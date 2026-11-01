using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Mapster;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.GetByStatus;

public class GetBookingsByStatusConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<GetBookingsByStatusQuery>
{
	public async Task Consume(ConsumeContext<GetBookingsByStatusQuery> context)
	{
		var status = Enum.Parse<BookingStatusEnum>(context.Message.Status!, true);

		var bookings = await uowFactory.ExecuteAsync(
			uow => uow.Bookings.GetByStatusAsync(status),
			context.CancellationToken);

		var dtos = bookings.Adapt<List<BookingDto>>();

		await context.RespondAsync(new BookingsListResponse(dtos));
	}
}