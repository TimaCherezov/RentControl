using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Mapster;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Locations.Create;

public class CreateLocationConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<CreateLocationCommand>
{
	public async Task Consume(ConsumeContext<CreateLocationCommand> context)
	{
		var command = context.Message;

		var location = await uowFactory.ExecuteAsync(uow =>
		{
			var locationModel = command.Adapt<Location>();

			uow.Locations.Add(locationModel);

			return Task.FromResult(locationModel);
		}, context.CancellationToken);

		var response = location.Adapt<LocationDto>();

		await context.RespondAsync(response);
	}
}
