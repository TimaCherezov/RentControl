using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Confirm;

public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
