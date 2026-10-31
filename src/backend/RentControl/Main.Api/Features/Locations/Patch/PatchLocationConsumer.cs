using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Locations.Patch;

public class PatchLocationConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<PatchLocationCommand>
{
    public async Task Consume(ConsumeContext<PatchLocationCommand> context)
    {
        var command = context.Message;

        await uowFactory.ExecuteAsync(async uow =>
        {
            var location = await uow.Locations.GetByIdAsync(command.Id, context.CancellationToken);
            if (location is null)
                throw new NotFoundException($"Location with ID:'{command.Id}' not found");

            if (command.Name is not null)
                location.Name = command.Name;
            if (command.Address is not null)
                location.Address = command.Address;

            uow.Locations.Update(location);
        }, context.CancellationToken);
    }
}
