using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.ProductImages.GetAll;

public class GetAllProductImageCommandValidator : AbstractValidator<GetAllProductImageCommand>
{
    public GetAllProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).Id();
    }
}
