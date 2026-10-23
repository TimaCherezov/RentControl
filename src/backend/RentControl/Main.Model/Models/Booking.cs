namespace Main.Model.Models;

public class Booking
{
    public Guid Id { get; set; }
    public required ProductUnit ProductUnit { get; set; }
    public Guid ProductUnitId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public required Location Location { get; set; }
    public Guid LocationId { get; set; }
    public TimeSpan? ActualReturnDate { get; set; }
    public decimal? TotalRetnalCost { get; set; }
    public decimal DepositAmount { get; set; }
    public required BookingStatus BookingStatus { get; set; }
    public Guid BookingStatusId { get; set; }
    public required User User { get; set; }
    public Guid UserId { get; set; }
}
