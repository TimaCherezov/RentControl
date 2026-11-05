using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Overdue;

public class OverdueBookingCommandValidator : AbstractValidator<OverdueBookingCommand>
{
    public OverdueBookingCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
