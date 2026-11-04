using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.Delete;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
