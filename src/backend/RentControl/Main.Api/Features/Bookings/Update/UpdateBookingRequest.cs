using Main.Model.Models;

namespace Main.Api.Features.Bookings.Update;

public sealed class UpdateBookingRequest
{
    public Guid UserId { get; set; }
    public Guid ProductUnitId { get; set; }
    public Guid LocationId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public DateTimeOffset? ActualReturnAt { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal? TotalRentalCost { get; set; }
    public BookingStatusEnum BookingStatus { get; set; }
    public DateTimeOffset StatusChangedAt { get; set; }
}
