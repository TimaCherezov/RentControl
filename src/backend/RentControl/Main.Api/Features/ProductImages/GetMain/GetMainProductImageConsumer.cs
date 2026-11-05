using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductImages.GetMain;

public class GetMainProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
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

        var response = image.Adapt<ProductImageDto>();
        await context.RespondAsync(response);
    }
}
