using FluentValidation;

namespace Main.Api.Features.Locations.GetPage;

public class GetPageLocationRequestValidator : AbstractValidator<GetPageLocationRequest>
{
	public GetPageLocationRequestValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).GreaterThan(0);
	}
}
