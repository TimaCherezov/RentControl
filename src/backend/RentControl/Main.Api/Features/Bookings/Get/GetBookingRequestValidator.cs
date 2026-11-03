using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Get;

public class GetBookingRequestValidator : AbstractValidator<GetBookingRequest>
{
    public GetBookingRequestValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
