using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductUnits.Get;

public class GetProductUnitCommandValidator : AbstractValidator<GetProductUnitCommand>
{
    public GetProductUnitCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
