using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductImages.Update;

public class UpdateProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<UpdateProductImageCommand>
{
    public async Task Consume(ConsumeContext<UpdateProductImageCommand> context)
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

                var wasMain = productImage.IsMain;

                command.Adapt(productImage);

                // NOTE: SetMainAsync использует ExecuteUpdateAsync (в обход change tracker);
                // важно, чтобы прямые UPDATE не пересекались
                // с отслеживаемой сущностью.
                if (productImage.IsMain)
                {
                    if (!wasMain)
                        await uow.ProductImages.SetMainAsync(
                            productImage.ProductId,
                            productImage.Id,
                            ct
                        );
                }
                else if (wasMain)
                {
                    var nextMainId = await uow.ProductImages.GetNextMainCandidateIdByProductIdAsync(
                        productImage.ProductId,
                        productImage.Id,
                        ct
                    );
                    if (nextMainId != null)
                    {
                        await uow.ProductImages.SetMainAsync(
                            productImage.ProductId,
                            nextMainId.Value,
                            ct
                        );
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Cannot unset IsMain for product image {productImage.Id}: "
                                + $"no other image of product {productImage.ProductId} is available to become the main one."
                        );
                    }
                }
            },
            ct
        );
    }
}
