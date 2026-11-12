using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Create;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.ProductUnitId).Id();
        RuleFor(x => x.LocationId).Id();
        RuleFor(x => x.DepositAmount).NonNegative();
        RuleFor(x => x.UserId).Id();
        RuleFor(x => x.EndAt).NotInPast();
        RuleFor(x => x.StartAt)
            .NotInPast()
            .LessThan(x => x.EndAt)
            .WithMessage("{PropertyName} must be before EndAt");
    }
}
