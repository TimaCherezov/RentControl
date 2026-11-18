using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.GetMaxAvailableDuration;

public class GetMaxAvailableDurationRequestValidator
    : AbstractValidator<GetMaxAvailableDurationRequest>
{
    public GetMaxAvailableDurationRequestValidator()
    {
        RuleFor(x => x.StartAt).NotInPast();
        RuleFor(x => x.LocationId).OptionalId();
    }
}
