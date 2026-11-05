using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductImages.GetMain;

public class GetMainProductImageCommandValidator : AbstractValidator<GetMainProductImageCommand>
{
    public GetMainProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).Id();
    }
}
