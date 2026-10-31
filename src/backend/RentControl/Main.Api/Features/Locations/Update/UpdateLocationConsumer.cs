using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Locations.Update;

public class UpdateLocationConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<UpdateLocationCommand>
{
    public async Task Consume(ConsumeContext<UpdateLocationCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var location = await uow.Locations.GetByIdAsync(command.Id, context.CancellationToken);
            if (location is null)
                throw new NotFoundException($"Location with ID:'{command.Id}' not found");

            location.Name = command.Name;
            location.Address = command.Address;

            uow.Locations.Update(location);
        }, context.CancellationToken);
    }
}
