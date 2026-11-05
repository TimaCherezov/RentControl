using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductUnits.Delete;

public class DeleteProductUnitConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<DeleteProductUnitCommand>
{
    public async Task Consume(ConsumeContext<DeleteProductUnitCommand> context)
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

                uow.ProductUnits.Remove(productUnit);
            },
            context.CancellationToken
        );
    }
}
