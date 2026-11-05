using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductUnits.Update;

public class UpdateProductUnitCommandValidator : AbstractValidator<UpdateProductUnitCommand>
{
    public UpdateProductUnitCommandValidator()
    {
        RuleFor(x => x.Id).Id();
        RuleFor(x => x.ProductId).Id();
        RuleFor(x => x.LocationId).Id();
        RuleFor(x => x.State).IsInEnum();
        RuleFor(x => x.SerialNumber).RequiredString(255);
        RuleFor(x => x.PurchaseDate).NotInFuture();
        RuleFor(x => x.PurchasePrice).NonNegative();
    }
}
