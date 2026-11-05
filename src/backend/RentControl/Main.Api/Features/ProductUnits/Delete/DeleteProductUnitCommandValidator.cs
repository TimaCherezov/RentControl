using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductUnits.Delete;

public class DeleteProductUnitCommandValidator : AbstractValidator<DeleteProductUnitCommand>
{
    public DeleteProductUnitCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
