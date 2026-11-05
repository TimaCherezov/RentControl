using Main.Model.Models;
using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductUnits.Create;

public class CreateProductUnitConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<CreateProductUnitCommand>
{
    public async Task Consume(ConsumeContext<CreateProductUnitCommand> context)
    {
        var command = context.Message;

        var productUnit = await uowFactory.ExecuteAsync(
            uow =>
            {
                var productUnitModel = command.Adapt<ProductUnit>();
                uow.ProductUnits.Add(productUnitModel);
                return Task.FromResult(productUnitModel);
            },
            context.CancellationToken
        );

        var response = productUnit.Adapt<ProductUnitDto>();

        await context.RespondAsync(response);
    }
}
