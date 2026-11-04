using FluentValidation;

namespace Main.Api.Features.Products.GetPage;

public class GetPageProductRequestValidator : AbstractValidator<GetPageProductRequest>
{
    public GetPageProductRequestValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
