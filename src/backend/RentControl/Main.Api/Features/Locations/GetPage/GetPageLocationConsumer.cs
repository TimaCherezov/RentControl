using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.DTOs;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Locations.GetPage;

public class GetPageLocationConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetPageLocationCommand>
{
    public async Task Consume(ConsumeContext<GetPageLocationCommand> context)
    {
        var command = context.Message;

        var locationPageList = await uowFactory.ExecuteAsync(async uow =>
        {
            var pageList = await uow.Locations.GetPageAsync(command.PageNumber, command.PageSize);
            return pageList;
        });

        var items = locationPageList.Adapt<List<LocationDto>>();
        var response = new PagedResponse<LocationDto>(
            items,
            locationPageList.TotalCount,
            locationPageList.PageNumber,
            locationPageList.PageSize
        );

        await context.RespondAsync(response);
    }
}
