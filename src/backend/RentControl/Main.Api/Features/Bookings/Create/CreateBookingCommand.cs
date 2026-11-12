namespace Main.Api.Features.Bookings.Create;

public record CreateBookingCommand(
    Guid UserId,
    Guid ProductUnitId,
    Guid LocationId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    decimal DepositAmount
);
