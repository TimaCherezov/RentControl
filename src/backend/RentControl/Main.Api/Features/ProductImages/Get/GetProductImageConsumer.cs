using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;
using Main.Api.Storage;
using Main.Api.Mappers;

namespace Main.Api.Features.ProductImages.Get;

public class GetProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, ImageStorageOptions imageStorageOptions)
    : IConsumer<GetProductImageCommand>
{
    public async Task Consume(ConsumeContext<GetProductImageCommand> context)
    {
        var command = context.Message;

        var productImage = await uowFactory.ExecuteAsync(
            async uow =>
            {
                var productImageModel =
                    await uow.ProductImages.GetByIdAsync(command.Id)
                    ?? throw new ObjectNotFoundException(
                        $"Product image with ID:'{command.Id}' not found"
                    );
                return productImageModel;
            },
            context.CancellationToken
        );

        var response = productImage.ToDto(imageStorageOptions);

        await context.RespondAsync(response);
    }
}
