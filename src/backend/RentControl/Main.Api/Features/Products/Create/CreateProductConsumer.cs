using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using Mapster;
using RentControl.Common.UnitOfWork;
using Main.Api.Mappers;
using Main.Api.Storage;
using Microsoft.Extensions.Options;
using Main.Api.Features.Bookings;

namespace Main.Api.Features.Products.Create;

public class CreateProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, ImageStorageOptions imageStorageOptions, IOptions<BookingOptions> bookingOptions) : IConsumer<CreateProductCommand>
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

		var response = product.ToDto(imageStorageOptions, bookingOptions.Value.DepositRate);

		await context.RespondAsync(response);
	}
}
