using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.Delete;

public class DeleteProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<DeleteProductCommand>
{
    public async Task Consume(ConsumeContext<DeleteProductCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var product = await uow.Products.GetByIdAsync(command.Id, context.CancellationToken);
            if (product is null)
                throw new ObjectNotFoundException($"Product with ID: {command.Id} not found.");
            uow.Products.Remove(product);
        }, context.CancellationToken);
    }
}
