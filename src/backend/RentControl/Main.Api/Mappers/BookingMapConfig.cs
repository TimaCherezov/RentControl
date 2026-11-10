using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.Create;
using Main.Api.Features.Bookings.GetPage;
using Main.Api.Features.Bookings.Update;
using Main.Api.Storage;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class BookingMapConfig
{
    public static BookingDto ToDto(this Booking b, ImageStorageOptions opt) =>
        new(
            b.Id,
            b.UserId,
            b.ProductUnitId,
            b.LocationId,
            b.StartAt,
            b.EndAt,
            b.ActualReturnAt,
            b.DepositAmount,
            b.TotalRentalCost,
            b.BookingStatus,
            b.StatusChangedAt,
            b.IsInvoiceCreated,
            ProductName: b.ProductUnit.Product.Name,
            MainImage: b.ProductUnit?.Product?.MainImage?.ToDto(opt)
        );

    public static List<BookingDto> ToDto(this IEnumerable<Booking> b, ImageStorageOptions opt) =>
        b.Select(b => b.ToDto(opt)).ToList();

    public static void Configure(TypeAdapterConfig config)
    {
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
