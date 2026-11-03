using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.GetPage;

public class GetPageBookingRequestValidator : AbstractValidator<GetPageBookingRequest>
{
    public GetPageBookingRequestValidator()
    {
        RuleFor(x => x.UserId).OptionalId();
        RuleFor(x => x.Status).OptionalIsInEnum();
        RuleFor(x => x.PageNumber).NonNegative();
        RuleFor(x => x.PageSize).NonNegative();
    }
}
