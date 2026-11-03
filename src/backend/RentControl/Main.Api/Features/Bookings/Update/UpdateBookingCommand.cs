using Main.Model.Models;

namespace Main.Api.Features.Bookings.Update;

public record UpdateBookingCommand(
    Guid Id,
    Guid UserId,
    Guid ProductUnitId,
    Guid LocationId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    DateTimeOffset? ActualReturnAt,
    decimal DepositAmount,
    decimal? TotalRentalCost,
    BookingStatusEnum BookingStatus,
    DateTimeOffset StatusChangedAt
);
