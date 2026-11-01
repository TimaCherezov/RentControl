using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.Update;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
	public UpdateProductRequestValidator()
	{
		RuleFor(x => x.Name).RequiredString(50);
		RuleFor(x => x.MarketValue).RequiredNonNegative();
		RuleFor(x => x.DepositOverride).OptionalNonNegative();
		RuleFor(x => x.DailyRate).RequiredNonNegative();
		RuleFor(x => x.HourlyRate).RequiredNonNegative();
		RuleFor(x => x.WeeklyRate).OptionalNonNegative();
		RuleFor(x => x.MonthlyRate).OptionalNonNegative();
		RuleFor(x => x.Description).OptionalString(1000);
	}
}