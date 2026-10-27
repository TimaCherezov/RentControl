namespace Main.Model.Models;

public class BookingStatus
{
    public BookingStatusEnum Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}

public enum BookingStatusEnum
{
    PENDING = 1,
    CANCELLED = 2,
    CONFIRMED = 3,
    READY_FOR_PICKUP = 4,
    ACTIVE = 5,
    OVERDUE = 6,
    RETURNED = 7,
    COMPLETED = 8,
}
