using Main.Api.Features.Bookings.Create;
using Main.Api.Features.Bookings.GetPage;
using Main.Api.Features.Bookings.Update;
using Main.Api.Features.Bookings;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class BookingMapConfig
{
    public static void Configure(TypeAdapterConfig config)
    {
        config
            .NewConfig<Booking, BookingDto>()
            .ConstructUsing(src => new BookingDto(
                src.Id,
                src.UserId,
                src.ProductUnitId,
                src.LocationId,
                src.StartAt,
                src.EndAt,
                src.ActualReturnAt,
                src.DepositAmount,
                src.TotalRentalCost,
                src.BookingStatus,
                src.StatusChangedAt
            ));

        config
            .NewConfig<CreateBookingRequest, CreateBookingCommand>()
            .ConstructUsing(src => new CreateBookingCommand(
                Guid.Empty,
                src.UserId,
                src.ProductUnitId,
                src.LocationId,
                src.StartAt,
                src.EndAt,
                src.ActualReturnAt,
                src.DepositAmount,
                src.TotalRentalCost,
                src.BookingStatus,
                src.StatusChangedAt
            ));

        config
            .NewConfig<UpdateBookingRequest, UpdateBookingCommand>()
            .ConstructUsing(src => new UpdateBookingCommand(
                Guid.Empty,
                src.UserId,
                src.ProductUnitId,
                src.LocationId,
                src.StartAt,
                src.EndAt,
                src.ActualReturnAt,
                src.DepositAmount,
                src.TotalRentalCost,
                src.BookingStatus,
                src.StatusChangedAt
            ));

        config.NewConfig<CreateBookingCommand, Booking>();

        config.NewConfig<UpdateBookingCommand, Booking>().Ignore(dest => dest.Id);

        config.NewConfig<GetPageBookingRequest, GetPageBookingCommand>();
    }
}
