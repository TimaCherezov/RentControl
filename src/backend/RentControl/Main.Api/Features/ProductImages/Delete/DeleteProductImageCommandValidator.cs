using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductImages.Delete;

public class DeleteProductImageCommandValidator : AbstractValidator<DeleteProductImageCommand>
{
    public DeleteProductImageCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
