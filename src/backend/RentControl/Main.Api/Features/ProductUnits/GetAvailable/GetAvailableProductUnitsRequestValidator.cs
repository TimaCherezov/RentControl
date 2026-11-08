using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductUnits.GetAvailable;

public class GetAvailableProductUnitsRequestValidator
    : AbstractValidator<GetAvailableProductUnitsRequest>
{
    public GetAvailableProductUnitsRequestValidator()
    {
        RuleFor(x => x.ProductId).Id();
        RuleFor(x => x.End).NotInPast();
        RuleFor(x => x.Start)
            .NotInPast()
            .LessThan(x => x.End)
            .WithMessage("{PropertyName} must be before End");
    }
}
