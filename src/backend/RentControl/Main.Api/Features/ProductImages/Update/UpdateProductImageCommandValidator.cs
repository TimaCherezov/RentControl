using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductImages.Update;

public class UpdateProductImageCommandValidator : AbstractValidator<UpdateProductImageCommand>
{
    public UpdateProductImageCommandValidator()
    {
        RuleFor(x => x.Id).Id();
        RuleFor(x => x.StorageKey).RequiredString(255);
        RuleFor(x => x.AltText).OptionalString(255);
    }
}
