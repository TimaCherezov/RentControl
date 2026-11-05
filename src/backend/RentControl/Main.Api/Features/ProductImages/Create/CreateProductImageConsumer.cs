using Main.Model.Models;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductImages.Create;

public class CreateProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<CreateProductImageCommand>
{
    public async Task Consume(ConsumeContext<CreateProductImageCommand> context)
    {
        var command = context.Message;

        var productImage = await uowFactory.ExecuteAsync(
            uow =>
            {
                var productImageModel = command.Adapt<ProductImage>();
                productImageModel.CreatedAt = DateTimeOffset.UtcNow;
                uow.ProductImages.Add(productImageModel);
                return Task.FromResult(productImageModel);
            },
            context.CancellationToken
        );

        var response = productImage.Adapt<ProductImageDto>();

        await context.RespondAsync(response);
    }
}
