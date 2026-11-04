using Main.Model.UnitOfWork;
using Mapster;
using MassTransit;
using RentControl.Common.DTOs;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.GetPage;

public class GetPageBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetPageBookingCommand>
{
    public async Task Consume(ConsumeContext<GetPageBookingCommand> context)
    {
        var command = context.Message;

        var bookingPageList = await uowFactory.ExecuteAsync(async uow =>
        {
            var pageList = await uow.Bookings.GetPageAsync(
                command.PageNumber,
                command.PageSize,
                command.UserId,
                command.Status
            );
            return pageList;
        });

        var items = bookingPageList.Adapt<List<BookingDto>>();
        var response = new PagedResponse<BookingDto>(
            items,
            bookingPageList.TotalCount,
            bookingPageList.PageNumber,
            bookingPageList.PageSize
        );

        await context.RespondAsync(response);
    }
}
