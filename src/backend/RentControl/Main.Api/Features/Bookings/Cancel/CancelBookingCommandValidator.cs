using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Cancel;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.Id).Id();
        RuleFor(x => x.UserId).Id();
    }
}
