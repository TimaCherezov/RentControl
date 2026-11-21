namespace Main.Api.Features.Bookings.Create;

public record CreateBookingCommand(
    Guid ProductId,
    Guid? LocationId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt
);
