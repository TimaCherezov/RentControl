using Main.Model.Models;

public record BookingDto(
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
