using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.Get;

public class GetProductCommandValidator : AbstractValidator<GetProductCommand>
{
    public GetProductCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
