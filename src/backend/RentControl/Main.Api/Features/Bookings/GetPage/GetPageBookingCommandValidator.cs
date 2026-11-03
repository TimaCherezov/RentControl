using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.GetPage;

public class GetPageBookingCommandValidator : AbstractValidator<GetPageBookingCommand>
{
    public GetPageBookingCommandValidator()
    {
        RuleFor(x => x.UserId).OptionalId();
        RuleFor(x => x.Status).OptionalIsInEnum();
        RuleFor(x => x.PageNumber).NonNegative();
        RuleFor(x => x.PageSize).NonNegative();
    }
}
