namespace Main.Api.Features.Bookings.Dto;

public record BookingDto(
    Guid Id,
    Guid ProductUnitId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    Guid LocationId,
    DateTimeOffset? ActualReturnAt,
    decimal? TotalRentalCost,
    decimal DepositAmount,
    string BookingStatusCode,
    string BookingStatusName,
    DateTimeOffset StatusChangedAt,
    Guid UserId);
