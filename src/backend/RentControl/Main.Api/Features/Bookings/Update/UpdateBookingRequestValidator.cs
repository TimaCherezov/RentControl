using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Bookings.Update;

public class UpdateBookingRequestValidator : AbstractValidator<UpdateBookingRequest>
{
    public UpdateBookingRequestValidator()
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
        RuleFor(x => x.BookingStatus).IsInEnum();
        RuleFor(x => x.TotalRentalCost).OptionalNonNegative();
        RuleFor(x => x.ActualReturnAt).OptionalNotInPast();
    }
}
