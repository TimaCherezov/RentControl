using Main.Model.Models;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;
using Main.Api.Storage;
using Main.Api.Mappers;

namespace Main.Api.Features.ProductImages.Create;

public class CreateProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, ImageStorageOptions opt)
    : IConsumer<CreateProductImageCommand>
{
    public async Task Consume(ConsumeContext<CreateProductImageCommand> context)
    {
        var command = context.Message;

        //TODO: сохранять локально / где-то еще картинку ^ сервис для сохранения

        var productImage = await uowFactory.ExecuteAsync(
            uow =>
            {
                var productImageModel = command.Adapt<ProductImage>();
                uow.ProductImages.Add(productImageModel);
                return Task.FromResult(productImageModel);
            },
            context.CancellationToken
        );

        var response = productImage.ToDto(opt);

        await context.RespondAsync(response);
    }
}
