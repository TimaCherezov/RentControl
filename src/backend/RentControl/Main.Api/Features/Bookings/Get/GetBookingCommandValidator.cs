using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Get;

public class GetBookingCommandValidator : AbstractValidator<GetBookingCommand>
{
    public GetBookingCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
