using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.Get;

public class GetProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetProductCommand>
{
    public async Task Consume(ConsumeContext<GetProductCommand> context)
    {
        var command = context.Message;

        var product = await uowFactory.ExecuteAsync(
            async uow =>
            {
                var productModel =
                    await uow.Products.GetByIdAsync(command.Id)
                    ?? throw new ObjectNotFoundException(
                        $"Product with ID:'{command.Id}' not found"
                    );
                return productModel;
            },
            context.CancellationToken
        );

        var response = product.Adapt<ProductDto>();

        await context.RespondAsync(response);
    }
}
