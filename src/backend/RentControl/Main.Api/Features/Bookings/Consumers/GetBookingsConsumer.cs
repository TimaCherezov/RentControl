using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.Dto;
using Main.Api.Features.Bookings.Queries;
using Main.Model.UnitOfWork;
using MassTransit;
using RentControl.Common.UnitOfWork;

namespace Main.Api.Features.Bookings.Consumers;

public class GetBookingsConsumer(IUnitOfWorkFactory<IMainUnitOfWork> uowFactory)
    : IConsumer<GetBookingsQuery>
{
    public async Task Consume(ConsumeContext<GetBookingsQuery> context)
    {
        var bookings = await uowFactory.ExecuteAsync(
            uow => uow.Bookings.GetListAsync(context.CancellationToken),
            context.CancellationToken);

        var dtos = bookings.Select(MapToDto).ToList();

        await context.RespondAsync(new BookingsListResponse(dtos));
    }

    private static BookingDto MapToDto(Main.Model.Models.Booking b) => new(
        b.Id,
        b.ProductUnitId,
        b.StartAt,
        b.EndAt,
        b.LocationId,
        b.ActualReturnAt,
        b.TotalRentalCost,
        b.DepositAmount,
        b.BookingStatus?.Code ?? string.Empty,
        b.BookingStatus?.Name ?? string.Empty,
        b.StatusChangedAt,
        b.UserId);
}
