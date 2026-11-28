using Main.Model.Models;
using RentControl.ServiceDefaults.Exceptions;

namespace Main.Api.Features.Bookings;

public class BookingStatusStateService : IBookingStatusStateService
{
    public Task<Booking> SwitchBookingStatusAsync(
        BookingStatusEnum targetStatus,
        Booking booking,
        CancellationToken ct
    )
    {
        var exception = new UnprocessableEntityException(
            $"Invalid booking status transition. Cannot change status for booking ID {booking.Id} from '{booking.BookingStatus}' to '{targetStatus}'."
        );
        var now = DateTimeOffset.UtcNow;

        switch (targetStatus)
        {
            case BookingStatusEnum.Confirmed:
                if (booking.BookingStatus != BookingStatusEnum.Pending)
                    throw exception;
                break;
            case BookingStatusEnum.Cancelled:
                if (booking.BookingStatus != BookingStatusEnum.Pending)
                    throw exception;
                break;
            case BookingStatusEnum.ReadyForPickup:
                if (booking.BookingStatus != BookingStatusEnum.Confirmed)
                    throw exception;
                break;
            case BookingStatusEnum.Active:
                if (booking.BookingStatus != BookingStatusEnum.ReadyForPickup)
                    throw exception;
                break;
            case BookingStatusEnum.Returned:
                if (
                    booking.BookingStatus != BookingStatusEnum.Active
                    && booking.BookingStatus != BookingStatusEnum.Overdue
                )
                    throw exception;
                booking.ActualReturnAt = now;
                break;
            case BookingStatusEnum.Overdue:
                if (booking.BookingStatus != BookingStatusEnum.Active)
                    throw exception;
                break;
            case BookingStatusEnum.Completed:
                if (booking.BookingStatus != BookingStatusEnum.Returned)
                    throw exception;
                break;
            default:
                throw new UnprocessableEntityException(
                    $"Invalid target status: '{targetStatus}'.");
        }

        booking.BookingStatus = targetStatus;
        booking.StatusChangedAt = now;

        return Task.FromResult(booking);
    }
}
