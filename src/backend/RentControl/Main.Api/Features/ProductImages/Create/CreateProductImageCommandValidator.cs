using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductImages.Create;

public class CreateProductImageCommandValidator : AbstractValidator<CreateProductImageCommand>
{
    public CreateProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).Id();
        RuleFor(x => x.StorageKey).RequiredString(255);
        RuleFor(x => x.AltText).OptionalString(255);
    }
}
