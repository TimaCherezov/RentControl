using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.DTOs;
using RentControl.Common.UnitOfWork;
using Main.Api.Storage;
using Main.Api.Mappers;

namespace Main.Api.Features.Bookings.GetPage;

public class GetPageBookingConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory, ImageStorageOptions opt)
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

        var items = bookingPageList.ToDto(opt);
        var response = new PagedResponse<BookingDto>(
            items,
            bookingPageList.TotalCount,
            bookingPageList.PageNumber,
            bookingPageList.PageSize
        );

        await context.RespondAsync(response);
    }
}
