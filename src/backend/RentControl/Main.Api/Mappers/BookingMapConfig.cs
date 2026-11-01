using Main.Api.Features.Bookings.Dto;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class BookingMapConfig
{
	public static void Configure(TypeAdapterConfig config)
	{
		config.NewConfig<Booking, BookingDto>()
			.ConstructUsing(src => new BookingDto(
				src.Id,
				src.ProductUnitId,
				src.StartAt,
				src.EndAt,
				src.LocationId,
				src.ActualReturnAt,
				src.TotalRentalCost,
				src.DepositAmount,
				src.BookingStatus.ToString(),
				src.StatusChangedAt,
				src.UserId
				)
			);
	}
}