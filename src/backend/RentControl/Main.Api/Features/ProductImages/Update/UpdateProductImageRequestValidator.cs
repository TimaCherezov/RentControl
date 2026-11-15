using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductImages.Update;

public class UpdateProductImageRequestValidator : AbstractValidator<UpdateProductImageRequest>
{
    public UpdateProductImageRequestValidator()
    {
        RuleFor(x => x.StorageKey).RequiredString(255);
        RuleFor(x => x.AltText).OptionalString(255);
    }
}
