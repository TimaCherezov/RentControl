using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.DTOs;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Products.GetPage;

public class GetPageProductConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetPageProductCommand>
{
    public async Task Consume(ConsumeContext<GetPageProductCommand> context)
    {
        var command = context.Message;

        var productPageList = await uowFactory.ExecuteAsync(async uow =>
        {
            var pageList = await uow.Products.GetPageAsync(command.PageNumber, command.PageSize);
            return pageList;
        });

        var items = productPageList.Adapt<List<ProductDto>>();
        var response = new PagedResponse<ProductDto>(
            items,
            productPageList.TotalCount,
            productPageList.PageNumber,
            productPageList.PageSize
        );

        await context.RespondAsync(response);
    }
}
