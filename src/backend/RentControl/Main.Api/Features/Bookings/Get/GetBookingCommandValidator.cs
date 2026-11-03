using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Get;

public class GetBookingValidator : AbstractValidator<GetBookingCommand>
{
    public GetBookingValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
