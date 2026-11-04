using FluentValidation;

namespace Main.Api.Features.Products.GetPage;

public class GetPageProductCommandValidator : AbstractValidator<GetPageProductCommand>
{
    public GetPageProductCommandValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
