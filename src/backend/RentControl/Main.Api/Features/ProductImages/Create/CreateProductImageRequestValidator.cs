using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductImages.Create;

public class CreateProductImageRequestValidator : AbstractValidator<CreateProductImageRequest>
{
    public CreateProductImageRequestValidator()
    {
        RuleFor(x => x.ProductId).Id();
        RuleFor(x => x.StorageKey).RequiredString(255);
        RuleFor(x => x.AltText).OptionalString(255);
    }
}
