using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductUnits.Update;

public class UpdateProductUnitRequestValidator : AbstractValidator<UpdateProductUnitRequest>
{
    public UpdateProductUnitRequestValidator()
    {
        RuleFor(x => x.ProductId).Id();
        RuleFor(x => x.LocationId).Id();
        RuleFor(x => x.State).IsInEnum();
        RuleFor(x => x.SerialNumber).RequiredString(255);
        RuleFor(x => x.PurchaseDate).NotInFuture();
        RuleFor(x => x.PurchasePrice).NonNegative();
    }
}
