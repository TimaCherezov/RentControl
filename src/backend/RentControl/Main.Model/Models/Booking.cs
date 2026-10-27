namespace Main.Model.Models;

public class Booking
{
    public Guid Id { get; set; }
    public ProductUnit ProductUnit { get; set; } = null!;
    public Guid ProductUnitId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public Location Location { get; set; } = null!;
    public Guid LocationId { get; set; }
    public DateTimeOffset? ActualReturnAt { get; set; }
    public decimal? TotalRentalCost { get; set; }
    public decimal DepositAmount { get; set; }
    public BookingStatus BookingStatus { get; set; } = null!;
    public DateTimeOffset StatusChangedAt { get; set; }
    public Guid BookingStatusId { get; set; }
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
}
