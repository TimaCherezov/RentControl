using Main.Model.Models;

namespace Main.Api.Features.Bookings.GetPage;

public sealed class GetPageBookingRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? UserId { get; set; }
    public BookingStatusEnum? Status { get; set; }
}
