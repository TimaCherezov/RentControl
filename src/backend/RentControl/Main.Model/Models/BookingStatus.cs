namespace Main.Model.Models;

/// <summary>
/// Статус бронирования.
/// </summary>
public enum BookingStatusEnum
{
    Pending = 1,
    Cancelled = 2,
    Confirmed = 3,
    ReadyForPickup = 4,
    Active = 5,
    Overdue = 6,
    Returned = 7,
    Completed = 8,
}
