using Main.Model.Models;
using Main.Model.UnitOfWork;
using Main.Api.Exceptions;
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
            async uow =>
            {
                var existing = await uow.ProductUnits.GetByProductAndSerialNumberAsync(
                    command.ProductId,
                    command.SerialNumber,
                    context.CancellationToken);
                if (existing is not null)
                {
                    throw new ConflictException(
                        $"Product unit with serial number '{command.SerialNumber}' already exists for product with ID: {command.ProductId}.");
                }

                var productUnitModel = command.Adapt<ProductUnit>();
                uow.ProductUnits.Add(productUnitModel);
                return productUnitModel;
            },
            context.CancellationToken
        );

        var response = productUnit.Adapt<ProductUnitDto>();

        await context.RespondAsync(response);
    }
}
