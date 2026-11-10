using FluentValidation;

namespace Main.Api.Features.Locations.GetPage;

public class GetPageLocationCommandValidator : AbstractValidator<GetPageLocationCommand>
{
	public GetPageLocationCommandValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).GreaterThan(0);
	}
}
