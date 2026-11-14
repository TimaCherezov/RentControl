namespace Main.Api.Features.Bookings.Create;

public record CreateBookingCommand(
    Guid UserId,
    Guid ProductId,
    Guid? LocationId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt
);
