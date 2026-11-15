using FluentValidation;
using Main.Api.Features.Bookings;
using Main.Api.Features.Validation;
using Microsoft.Extensions.Options;

namespace Main.Api.Features.Products.GetQuote;

public class GetQuoteProductRequestValidator
    : AbstractValidator<GetQuoteProductRequest>
{
    public GetQuoteProductRequestValidator(IOptions<BookingOptions> bookingOptions)
    {
        var maxDuration = bookingOptions.Value.MaxBookingDuration;

        RuleFor(x => x.StartAt).NotInPast();
        RuleFor(x => x.EndAt)
            .GreaterThan(x => x.StartAt)
            .WithMessage("EndAt must be after StartAt");
        RuleFor(x => x.EndAt - x.StartAt).LessThanOrEqualTo(maxDuration);
    }
}