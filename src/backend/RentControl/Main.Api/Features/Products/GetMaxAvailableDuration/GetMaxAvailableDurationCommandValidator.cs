using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.GetMaxAvailableDuration;

public class GetMaxAvailableDurationCommandValidator
    : AbstractValidator<GetMaxAvailableDurationCommand>
{
    public GetMaxAvailableDurationCommandValidator()
    {
        RuleFor(x => x.Id).Id();
        RuleFor(x => x.StartAt).NotInPast();
        RuleFor(x => x.LocationId).OptionalId();
    }
}
