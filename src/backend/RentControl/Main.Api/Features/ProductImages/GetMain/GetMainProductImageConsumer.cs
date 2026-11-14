using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;
using Main.Api.Mappers;
using Microsoft.Extensions.Options;

namespace Main.Api.Features.ProductImages.GetMain;

public class GetMainProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, IOptions<ImageStorageOptions> imageStorageOptions)
    : IConsumer<GetMainProductImageCommand>
{
    public async Task Consume(ConsumeContext<GetMainProductImageCommand> context)
    {
        var command = context.Message;

        var image = await uowFactory.ExecuteAsync(
            async uow =>
            {
                var imageModel =
                    await uow.ProductImages.GetMainByProductIdAsync(command.ProductId)
                    ?? throw new ObjectNotFoundException(
                        $"Main image for product '{command.ProductId}' not found"
                    );
                return imageModel;
            },
            context.CancellationToken
        );

        var response = image.ToDto(imageStorageOptions.Value);
        await context.RespondAsync(response);
    }
}
