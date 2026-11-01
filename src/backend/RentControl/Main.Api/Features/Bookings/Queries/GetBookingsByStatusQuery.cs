using Main.Model.Models;

namespace Main.Api.Features.Bookings.Queries;

public record GetBookingsByStatusQuery(BookingStatusEnum Status);