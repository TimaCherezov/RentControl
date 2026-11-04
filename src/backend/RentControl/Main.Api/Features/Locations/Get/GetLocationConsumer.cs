using Main.Api.Exceptions;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Locations.Get;

public class GetLocationConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetLocationCommand>
{
    public async Task Consume(ConsumeContext<GetLocationCommand> context)
    {
        var command = context.Message;

        var location = await uowFactory.ExecuteAsync(
            async uow =>
            {
                var locationModel =
                    await uow.Locations.GetByIdAsync(command.Id)
                    ?? throw new ObjectNotFoundException(
                        $"Location with ID:'{command.Id}' not found"
                    );
                return locationModel;
            },
            context.CancellationToken
        );

        var response = location.Adapt<LocationDto>();

        await context.RespondAsync(response);
    }
}
