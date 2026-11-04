using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Issue;

public class IssueBookingCommandValidator : AbstractValidator<IssueBookingCommand>
{
    public IssueBookingCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
