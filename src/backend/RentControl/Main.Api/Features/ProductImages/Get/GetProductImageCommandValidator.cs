using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductImages.Get;

public class GetProductImageCommandValidator : AbstractValidator<GetProductImageCommand>
{
    public GetProductImageCommandValidator()
    {
        RuleFor(x => x.Id).Id();
    }
}
