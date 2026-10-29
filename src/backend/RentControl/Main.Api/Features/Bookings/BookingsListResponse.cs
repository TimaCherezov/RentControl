using Main.Api.Features.Bookings.Dto;

namespace Main.Api.Features.Bookings;

public record BookingsListResponse(List<BookingDto> Bookings);
