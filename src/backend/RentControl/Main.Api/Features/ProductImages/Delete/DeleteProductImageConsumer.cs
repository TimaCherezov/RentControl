using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductImages.Delete;

public class DeleteProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<DeleteProductImageCommand>
{
    public async Task Consume(ConsumeContext<DeleteProductImageCommand> context)
    {
        var command = context.Message;
        var ct = context.CancellationToken;

        await uowFactory.ExecuteAsync(
            async uow =>
            {
                var productImage = await uow.ProductImages.GetByIdAsync(command.Id, ct);
                if (productImage is null)
                    throw new ObjectNotFoundException(
                        $"Product image with ID: {command.Id} not found."
                    );

                uow.ProductImages.Remove(productImage);

                if (productImage.IsMain)
                {
                    var nextMainId = await uow.ProductImages.GetNextMainCandidateIdByProductIdAsync(
                        productImage.ProductId,
                        productImage.Id,
                        ct
                    );
                    if (nextMainId != null)
                        await uow.ProductImages.SetMainAsync(
                            productImage.ProductId,
                            nextMainId.Value,
                            ct
                        );
                }
            },
            ct
        );
    }
}
