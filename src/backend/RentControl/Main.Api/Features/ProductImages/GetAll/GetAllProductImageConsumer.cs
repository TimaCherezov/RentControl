using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.ProductImages.GetAll;

public class GetAllProductImageConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
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

        var response = list.Adapt<List<ProductImageDto>>();
        await context.RespondAsync(response);
    }
}
