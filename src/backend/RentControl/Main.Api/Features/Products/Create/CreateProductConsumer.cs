using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Mapster;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.Create;

public class CreateProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<CreateProductCommand>
{
	public async Task Consume(ConsumeContext<CreateProductCommand> context)
	{
		var command = context.Message;

		var product = await uowFactory.ExecuteAsync(uow =>
		{
			var productModel = command.Adapt<Product>();

			uow.Products.Add(productModel);

			return Task.FromResult(productModel);
		}, context.CancellationToken);

		var response = product.Adapt<ProductDto>();

		await context.RespondAsync(response);
	}
}
