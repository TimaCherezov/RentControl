using Main.Model.Models;

namespace Main.Api.Features.Bookings.GetPage;

public record GetPageBookingCommand(
    int PageNumber,
    int PageSize,
    BookingStatusEnum? Status,
    Guid UserId
);
