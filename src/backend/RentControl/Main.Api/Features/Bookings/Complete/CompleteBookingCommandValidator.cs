using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Complete;

public class CompleteBookingCommandValidator : AbstractValidator<CompleteBookingCommand>
{
    public CompleteBookingCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
