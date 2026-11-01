using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.Create;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
	public CreateProductCommandValidator()
	{
		RuleFor(x => x.Name).RequiredString(50);
		RuleFor(x => x.MarketValue).NonNegative();
		RuleFor(x => x.DepositOverride).OptionalNonNegative();
		RuleFor(x => x.DailyRate).NonNegative();
		RuleFor(x => x.HourlyRate).NonNegative();
		RuleFor(x => x.WeeklyRate).OptionalNonNegative();
		RuleFor(x => x.MonthlyRate).OptionalNonNegative();
		RuleFor(x => x.Description).OptionalString(1000);
	}
}