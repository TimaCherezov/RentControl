using FluentValidation;
using Main.Api.Features.Validation;
using Microsoft.Extensions.Options;

namespace Main.Api.Features.Bookings.Create;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator(IOptions<BookingOptions> bookingOptions)
    {
        var maxDuration = bookingOptions.Value.MaxBookingDuration;
        RuleFor(x => x.ProductId).Id();
        RuleFor(x => x.LocationId).OptionalId();
        RuleFor(x => x.UserId).Id();
        RuleFor(x => x.EndAt).NotInPast();
        RuleFor(x => x.StartAt)
            .NotInPast()
            .LessThan(x => x.EndAt)
            .WithMessage("{PropertyName} must be before EndAt");
        RuleFor(x => x.EndAt - x.StartAt).LessThanOrEqualTo(maxDuration);
    }
}
