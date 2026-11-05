using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductUnits.Get;

public class GetProductUnitConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetProductUnitCommand>
{
    public async Task Consume(ConsumeContext<GetProductUnitCommand> context)
    {
        var command = context.Message;

        var productUnit = await uowFactory.ExecuteAsync(
            async uow =>
            {
                var productUnitModel =
                    await uow.ProductUnits.GetByIdAsync(command.Id)
                    ?? throw new ObjectNotFoundException(
                        $"Product unit with ID:'{command.Id}' not found"
                    );
                return productUnitModel;
            },
            context.CancellationToken
        );

        var response = productUnit.Adapt<ProductUnitDto>();

        await context.RespondAsync(response);
    }
}
