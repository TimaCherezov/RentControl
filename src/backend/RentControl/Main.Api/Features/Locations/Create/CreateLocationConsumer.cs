using Main.Model.Models;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Locations.Create;

public class CreateLocationConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory) : IConsumer<CreateLocationCommand>
{
    public async Task Consume(ConsumeContext<CreateLocationCommand> context)
    {
        var command = context.Message;

        var location = await uowFactory.ExecuteAsync(async uow =>
        {
            var locationModel = new Location
            {
                Name = command.Name,
                Address = command.Address
            };

            await uow.Locations.AddAsync(locationModel);

            return locationModel;
        }, context.CancellationToken);

        var response = new LocationDto(location.Id, location.Name, location.Address);

        await context.RespondAsync(response);
    }
}
