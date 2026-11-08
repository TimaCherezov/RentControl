using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductUnits.GetAvailable;

public class GetAvailableProductUnitsConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetAvailableProductUnitsCommand>
{
    public async Task Consume(ConsumeContext<GetAvailableProductUnitsCommand> context)
    {
        var command = context.Message;

        var units = await uowFactory.ExecuteAsync(
            async uow =>
            {
                return await uow.ProductUnits.GetAvailableAsync(
                    command.ProductId,
                    command.Start,
                    command.End,
                    context.CancellationToken
                );
            },
            context.CancellationToken
        );

        var response = units.Adapt<List<ProductUnitDto>>();
        await context.RespondAsync(response);
    }
}
