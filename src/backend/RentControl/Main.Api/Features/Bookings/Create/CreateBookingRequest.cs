namespace Main.Api.Features.Bookings.Create;

public sealed class CreateBookingRequest
{
    public Guid ProductId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public Guid? LocationId { get; set; }
    public Guid UserId { get; set; }
}
