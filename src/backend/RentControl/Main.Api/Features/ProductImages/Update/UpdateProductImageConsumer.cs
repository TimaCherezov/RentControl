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

        await uowFactory.ExecuteAsync(
            async uow =>
            {
                var productImage = await uow.ProductImages.GetByIdAsync(
                    command.Id,
                    context.CancellationToken
                );
                if (productImage is null)
                    throw new ObjectNotFoundException(
                        $"Product image with ID: {command.Id} not found."
                    );

                command.Adapt(productImage);
            },
            context.CancellationToken
        );
    }
}
