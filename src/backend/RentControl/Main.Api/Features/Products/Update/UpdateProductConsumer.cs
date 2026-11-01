using Main.Api.Exceptions;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Mapster;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.Update;

public class UpdateProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<UpdateProductCommand>
{
	public async Task Consume(ConsumeContext<UpdateProductCommand> context)
	{
		var command = context.Message;

		await uowFactory.ExecuteAsync(async uow =>
		{
			var product = await uow.Products.GetByIdAsync(command.Id, context.CancellationToken);
			if (product is null)
				throw new ObjectNotFoundException($"Product with ID:'{command.Id}' not found");

			command.Adapt(product);
		}, context.CancellationToken);
	}
}
