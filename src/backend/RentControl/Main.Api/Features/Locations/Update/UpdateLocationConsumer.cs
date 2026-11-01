using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using Mapster;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Locations.Update;

public class UpdateLocationConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<UpdateLocationCommand>
{
	public async Task Consume(ConsumeContext<UpdateLocationCommand> context)
	{
		var command = context.Message;

		await uowFactory.ExecuteAsync(async uow =>
		{
			var location = await uow.Locations.GetByIdAsync(command.Id, context.CancellationToken);
			if (location is null)
				throw new ObjectNotFoundException($"Location with ID:'{command.Id}' not found");

			command.Adapt(location);
		}, context.CancellationToken);
	}
}