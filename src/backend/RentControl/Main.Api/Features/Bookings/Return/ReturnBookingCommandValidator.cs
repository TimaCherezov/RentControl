using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Return;

public class ReturnBookingCommandValidator : AbstractValidator<ReturnBookingCommand>
{
    public ReturnBookingCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
