namespace Main.Model.Models;

public static class BookingStatusExtensions
{
    public static readonly IReadOnlySet<BookingStatusEnum> BlockingStatuses =
        new HashSet<BookingStatusEnum>
        {
            BookingStatusEnum.Pending,
            BookingStatusEnum.Confirmed,
            BookingStatusEnum.ReadyForPickup,
            BookingStatusEnum.Active,
            BookingStatusEnum.Overdue,
        };

    public static bool IsBlocking(BookingStatusEnum status) => BlockingStatuses.Contains(status);
}
