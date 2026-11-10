using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;
using Main.Api.Storage;
using Main.Api.Mappers;

namespace Main.Api.Features.ProductImages.GetAll;

public class GetAllProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, ImageStorageOptions opt)
    : IConsumer<GetAllProductImageCommand>
{
    public async Task Consume(ConsumeContext<GetAllProductImageCommand> context)
    {
        var command = context.Message;

        var list = await uowFactory.ExecuteAsync(async uow =>
            await uow.ProductImages.GetAllByProductIdAsync(
                command.ProductId,
                context.CancellationToken
            )
        );

        var response = list.ToDto(opt);
        await context.RespondAsync(response);
    }
}
