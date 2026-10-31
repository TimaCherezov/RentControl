using FluentValidation;

namespace Main.Api.Features.Products.Update;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
	public UpdateProductRequestValidator()
	{
		RuleFor(x => x.Name)
			.Must(name => !string.IsNullOrWhiteSpace(name))
			.WithMessage("Name is required")
			.MaximumLength(50)
			.WithMessage("Name must be at most 50 characters");

		RuleFor(x => x.MarketValue)
			.NotNull()
			.WithMessage("MarketValue is required")
			.GreaterThanOrEqualTo(0)
			.WithMessage("MarketValue must be non-negative");

		RuleFor(x => x.DepositOverride)
			.GreaterThanOrEqualTo(0)
			.When(x => x.DepositOverride.HasValue)
			.WithMessage("DepositOverride must be non-negative");

		RuleFor(x => x.DailyRate)
			.NotNull()
			.WithMessage("DailyRate is required")
			.GreaterThanOrEqualTo(0)
			.WithMessage("DailyRate must be non-negative");

		RuleFor(x => x.HourlyRate)
			.NotNull()
			.WithMessage("HourlyRate is required")
			.GreaterThanOrEqualTo(0)
			.WithMessage("HourlyRate must be non-negative");

		RuleFor(x => x.WeeklyRate)
			.GreaterThanOrEqualTo(0)
			.When(x => x.WeeklyRate.HasValue)
			.WithMessage("WeeklyRate must be non-negative");

		RuleFor(x => x.MonthlyRate)
			.GreaterThanOrEqualTo(0)
			.When(x => x.MonthlyRate.HasValue)
			.WithMessage("MonthlyRate must be non-negative");

		RuleFor(x => x.Description)
			.MaximumLength(1000)
			.When(x => x.Description is not null)
			.WithMessage("Description must be at most 1000 characters");
	}
}