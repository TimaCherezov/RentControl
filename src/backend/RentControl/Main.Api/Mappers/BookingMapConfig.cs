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
        new BookingDto(
            b.Id,
            b.Location?.Name,
            b.Location?.Address,
            b.StartAt,
            b.EndAt,
            b.DepositAmount,
            b.BookingStatus,
            b.StatusChangedAt,
            b.ProductUnit?.Product?.Name,
            b.ProductUnit?.ProductId,
            b.ProductUnit?.Product?.MainImage?.ToDto(opt),
            b.ActualReturnAt,
            b.TotalRentalCost,
            b.IsInvoiceCreated
        );

    public static List<BookingDto> ToDto(this IEnumerable<Booking> b, ImageStorageOptions opt) =>
        b.Select(b => b.ToDto(opt)).ToList();

    public static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<CreateBookingRequest, CreateBookingCommand>();

        config.NewConfig<UpdateBookingRequest, UpdateBookingCommand>();

        config.NewConfig<CreateBookingCommand, Booking>();

        config.NewConfig<UpdateBookingCommand, Booking>().Ignore(dest => dest.Id);

        config.NewConfig<GetPageBookingRequest, GetPageBookingCommand>();
    }
}
