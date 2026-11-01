using Main.Api.Features.Bookings.Dto;
using Main.Api.Features.Bookings.Queries;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Mapster;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Consumers;

public class GetBookingsByStatusConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<GetBookingsByStatusQuery>
{
	public async Task Consume(ConsumeContext<GetBookingsByStatusQuery> context)
	{
		var bookings = await uowFactory.ExecuteAsync(
			uow => uow.Bookings.GetByStatusAsync(
				context.Message.Status,
				context.CancellationToken),
			context.CancellationToken);

		var dtos = bookings.Adapt<List<BookingDto>>();

		await context.RespondAsync(new BookingsListResponse(dtos));
	}
}