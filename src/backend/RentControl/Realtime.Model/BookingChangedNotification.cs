namespace Realtime.Model;

public record BookingChangedNotification(
    Guid BookingId,
    string Status);
