using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Locations.Delete;

public class DeleteLocationConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<DeleteLocationCommand>
{
    public async Task Consume(ConsumeContext<DeleteLocationCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var location = await uow.Locations.GetByIdAsync(command.Id, context.CancellationToken);
            if (location is null)
                throw new ObjectNotFoundException($"Location with ID: {command.Id} not found.");
            uow.Locations.Remove(location);
        }, context.CancellationToken);
    }
}
