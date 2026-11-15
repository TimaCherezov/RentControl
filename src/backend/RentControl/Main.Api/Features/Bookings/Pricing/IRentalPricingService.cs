using Main.Model.Models;

namespace Main.Api.Features.Bookings.Pricing;

public interface IRentalPricingService
{
    BookingQuote Calculate(Product product, DateTimeOffset startAt, DateTimeOffset endAt);
}