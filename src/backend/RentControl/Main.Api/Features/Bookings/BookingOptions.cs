namespace Main.Api.Features.Bookings;

public sealed class BookingOptions
{
    public const string Section = "Booking";

    public TimeSpan MaxBookingDuration { get; set; } = TimeSpan.FromDays(365);

    public decimal DepositRate { get; set; } = 0.3m;
}
