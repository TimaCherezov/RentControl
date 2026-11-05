using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductUnits.Update;

public class UpdateProductUnitConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<UpdateProductUnitCommand>
{
    public async Task Consume(ConsumeContext<UpdateProductUnitCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(
            async uow =>
            {
                var productUnit = await uow.ProductUnits.GetByIdAsync(
                    command.Id,
                    context.CancellationToken
                );
                if (productUnit is null)
                    throw new ObjectNotFoundException(
                        $"Product unit with ID: {command.Id} not found."
                    );

                command.Adapt(productUnit);
            },
            context.CancellationToken
        );
    }
}
