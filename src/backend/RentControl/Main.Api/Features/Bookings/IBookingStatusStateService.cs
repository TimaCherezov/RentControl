using Main.Model.Models;

namespace Main.Api.Features.Bookings;

public interface IBookingStatusStateService
{
    Task<Booking> SwitchBookingStatusAsync(
        BookingStatusEnum targetStatus,
        Booking booking,
        CancellationToken ct
    );
}
