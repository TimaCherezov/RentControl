using Main.Api.Exceptions;
using Main.Api.Mappers;
using Main.Model.Models;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using Microsoft.Extensions.Options;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductImages.Create;

public class CreateProductImageConsumer(
    IUnitOfWorkFactory<IMainUnitOfWork> uowFactory,
    IOptions<ImageStorageOptions> imageStorageOptions
) : IConsumer<CreateProductImageCommand>
{
    public async Task Consume(ConsumeContext<CreateProductImageCommand> context)
    {
        var command = context.Message;
        var ct = context.CancellationToken;

        //TODO: сохранять локально / где-то еще картинку ^ сервис для сохранения

        var productImage = await uowFactory.ExecuteAsync(
            async uow =>
            {
                _ =
                    await uow.Products.GetAndLockForUpdateByIdAsync(command.ProductId, ct)
                    ?? throw new ObjectNotFoundException($"Product with ID: {command.ProductId} not found.");

                var productImageModel = command.Adapt<ProductImage>();

                var hasMain =
                    await uow.ProductImages.GetMainByProductIdAsync(productImageModel.ProductId, ct)
                    != null;

                productImageModel.IsMain = !hasMain;
                uow.ProductImages.Add(productImageModel);
                return productImageModel;
            },
            ct
        );

        var response = productImage.ToDto(imageStorageOptions.Value);

        await context.RespondAsync(response);
    }
}
