namespace Main.Model.Models;

public class Booking
{
    public Guid Id { get; set; }
    public required ProductUnit ProductUnit { get; set; }
    public Guid ProductUnitId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public required Location Location { get; set; }
    public Guid LocationId { get; set; }
    public DateTimeOffset? ActualReturnAt { get; set; }
    public decimal? TotalRentalCost { get; set; }
    public decimal DepositAmount { get; set; }
    public required BookingStatus BookingStatus { get; set; }
    public DateTimeOffset StatusChangedAt { get; set; }
    public Guid BookingStatusId { get; set; }
    public required User User { get; set; }
    public Guid UserId { get; set; }
}
